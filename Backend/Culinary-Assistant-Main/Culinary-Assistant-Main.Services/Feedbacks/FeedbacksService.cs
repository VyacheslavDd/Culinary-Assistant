using Core.Base;
using Core.Base.Interfaces;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Feedback;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.ReceiptRates;
using Microsoft.EntityFrameworkCore;
using Minio;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Feedbacks
{
	public class FeedbacksService(IRatesRepository<ReceiptRate, Receipt> ratesRepository, IUsersRepository usersRepository, IReceiptsRepository receiptsRepository, IMinioClientFactory minioClientFactory,
		IFeedbacksRepository repository, ILogger logger) : BaseService<Feedback, FeedbackInDTO, FeedbackUpdateDTO>(repository, logger), IFeedbacksService
	{
		private readonly IRatesRepository<ReceiptRate, Receipt> _ratesRepository = ratesRepository;
		private readonly IUsersRepository _usersRepository = usersRepository;
		private readonly IReceiptsRepository _receiptsRepository = receiptsRepository;
		private readonly IMinioClientFactory _minioClientFactory = minioClientFactory;

		private readonly Dictionary<FeedbacksSortOption?, Func<Feedback, double>> _orderByExpressions = new()
		{
			{ FeedbacksSortOption.ByDate, (Feedback feedback) => feedback.UpdatedAt.Ticks },
			{ FeedbacksSortOption.ByRate, (Feedback feedback) => feedback.Rate == null ? 0 : feedback.Rate.Value }
		};

		public override async Task<Result<Guid>> CreateAsync(FeedbackInDTO entityCreateRequest, bool autoSave = true)
		{
			var user = await _usersRepository.GetBySelectorAsync(u => u.Id == entityCreateRequest.UserId);
			if (user == null) return Result.Failure<Guid>("Несуществующий пользователь не может написать отзыв");
			var receipt = await _receiptsRepository.GetBySelectorAsync(r => r.Id == entityCreateRequest.ReceiptId);
			if (receipt == null) return Result.Failure<Guid>("Невозможно отправить отзыв к несуществующему рецепту");
			var feedbackRes = Feedback.Create(entityCreateRequest);
			if (feedbackRes.IsFailure) return Result.Failure<Guid>(feedbackRes.Error);
			var guidRes = await AddToRepositoryAsync(feedbackRes);
			return guidRes;
		}

		public async Task<EntitiesResponseWithCountAndPages<Feedback>> GetAllAsync(Guid receiptId, FeedbacksFilter feedbacksFilter, CancellationToken cancellationToken = default)
		{
			var feedbacksCount = await _repository.GetAll().Where(f => f.ReceiptId == receiptId).CountAsync();
			var pagesCount = (int)Math.Ceiling((double)feedbacksCount / feedbacksFilter.Limit);
			var feedbacks = await _repository
				.GetAll()
				.Where(f => f.ReceiptId == receiptId)
				.OrderByDescending(f => f.UpdatedAt)
				.Skip((feedbacksFilter.Page - 1) * feedbacksFilter.Limit)
				.Take(feedbacksFilter.Limit)
				.ToListAsync(cancellationToken);
			await EnrichFeedbacksWithRateAndUserDataAsync(feedbacks, cancellationToken);
			var sortedFeedbacks = DoSorting(feedbacks, feedbacksFilter.SortOption, _orderByExpressions, feedbacksFilter.IsAscendingSorting);
			return new EntitiesResponseWithCountAndPages<Feedback>(sortedFeedbacks, feedbacksCount, pagesCount);
		}

		public override async Task<Feedback?> GetByGuidAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var feedback = await base.GetByGuidAsync(id, cancellationToken);
			if (feedback == null) return feedback;
			await EnrichFeedbacksWithRateAndUserDataAsync([feedback], cancellationToken);
			return feedback;
		}

		public override async Task<Result> NotBulkUpdateAsync(Guid feedbackId, FeedbackUpdateDTO updateRequest)
		{
			var feedback = await _repository.GetBySelectorAsync(f => f.Id == feedbackId);
			if (feedback == null) return Result.Failure("Попытка обновить несуществующий отзыв");
			var textUpdateRes = feedback.SetText(updateRequest.Text);
			if (textUpdateRes.IsFailure) return Result.Failure(textUpdateRes.Error);
			return await base.NotBulkUpdateAsync(feedbackId, updateRequest);
		}

		private async Task EnrichFeedbacksWithRateAndUserDataAsync(List<Feedback> feedbacks, CancellationToken cancellationToken = default)
		{
			using var minioClient = _minioClientFactory.CreateClient();
			foreach (var feedback in feedbacks)
			{
				var rate = await _ratesRepository.GetAsync(feedback.UserId, feedback.ReceiptId, cancellationToken);
				feedback.Rate = rate?.Rating;
				var user = await _usersRepository.GetBySelectorAsync(u => u.Id == feedback.UserId);
				if (user == null) continue;
				feedback.UserLogin = user.Login.Value;
				var profilePicturePath = (await MinioUtils.GetPresignedUrlsForFilesFromFilePathsAsync(minioClient, _logger, [new FilePath(user.ProfilePictureUrl ?? "")]))[0];
				feedback.UserProfilePictureUrl = profilePicturePath.Url;
			}
		}
	}
}
