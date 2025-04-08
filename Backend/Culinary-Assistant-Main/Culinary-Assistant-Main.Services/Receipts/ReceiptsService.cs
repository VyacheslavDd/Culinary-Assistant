using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.Users;
using Culinary_Assistant.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using System.Text.Json;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Const;
using Minio;

namespace Culinary_Assistant_Main.Services.Receipts
{
	public class ReceiptsService(IUsersService usersService, IFileMessagesProducerService fileMessagesProducerService, IMinioClientFactory minioClientFactory,
		IReceiptsRepository receiptsRepository, ILogger logger) :
		BaseService<Receipt, ReceiptInDTO, UpdateReceiptDTO>(receiptsRepository, logger), IReceiptsService
	{
		private readonly IUsersService _usersService = usersService;
		private readonly IMinioClientFactory _minioClientFactory = minioClientFactory;
		private readonly IFileMessagesProducerService _fileMessagesProducerService = fileMessagesProducerService;

		public async Task<EntitiesResponseWithCountAndPages<Receipt>> GetAllAsync(ReceiptsFilter receiptsFilter, CancellationToken cancellationToken = default)
		{
			var filteredReceipts = await DoReceiptsFilteringAsync(receiptsFilter, cancellationToken);
			var dataCount = filteredReceipts.Count;
			var pagesCount = (int)Math.Ceiling((double)dataCount / receiptsFilter.Limit);
			var currentPage = filteredReceipts.Skip(receiptsFilter.Limit * (receiptsFilter.Page - 1))
										    .Take(receiptsFilter.Limit)
											.ToList();
			return new EntitiesResponseWithCountAndPages<Receipt>(currentPage, dataCount, pagesCount);
		}

		public async override Task<Receipt?> GetByGuidAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var receipt = await base.GetByGuidAsync(id, cancellationToken);
			if (receipt != null)
				await _repository.LoadReferenceAsync(receipt, r => r.User);
			return receipt;
		}

		public override async Task<Result> NotBulkUpdateAsync(Guid entityId, UpdateReceiptDTO updateRequest)
		{
			var results = Miscellaneous.CreateResultList(6);
			var existingReceipt = await GetByGuidAsync(entityId);
			if (existingReceipt == null) return Result.Failure("Сущности для обновления не существует");
			existingReceipt.SetCategory(updateRequest.Category ?? existingReceipt.Category);
			existingReceipt.SetCookingDifficulty(updateRequest.CookingDifficulty ?? existingReceipt.CookingDifficulty);
			if (updateRequest.Tags != null) existingReceipt.SetTags(updateRequest.Tags);
			results[0] = existingReceipt.SetTitle(updateRequest.Title ?? existingReceipt.Title.Value);
			results[1] = existingReceipt.SetDescription(updateRequest.Description ?? existingReceipt.Description.Value);
			results[2] = existingReceipt.SetCookingTime(updateRequest.CookingTime ?? existingReceipt.CookingTime);
			if (updateRequest.Ingredients != null)
				results[3] = existingReceipt.SetIngredients(updateRequest.Ingredients);

			if (updateRequest.CookingSteps != null)
				results[4] = existingReceipt.SetCookingSteps(updateRequest.CookingSteps);
			if (updateRequest.PicturesUrls != null)
				results[5] = existingReceipt.SetPictures(updateRequest.PicturesUrls);
			if (!results.All(r => r.IsSuccess)) return Miscellaneous.ResultFailureWithAllFailuresFromResultList(results);
			return await base.NotBulkUpdateAsync(entityId, updateRequest);
		}

		public override async Task<Result<Guid>> CreateAsync(ReceiptInDTO entityCreateRequest, bool autoSave = true)
		{
			var existingUser = await _usersService.GetByGuidAsync(entityCreateRequest.UserId);
			if (existingUser == null) return Result.Failure<Guid>($"Пользователя с Guid {entityCreateRequest.UserId} не существует");
			var receiptResult = Receipt.Create(entityCreateRequest);
			if (receiptResult.IsSuccess) receiptResult.Value.SetNutrients(20, 15, 20, 30);
			return await AddToRepositoryAsync(receiptResult);
		}

		public override async Task<Result<string>> NotBulkDeleteAsync(Guid entityId)
		{
			var entity = await GetByGuidAsync(entityId);
			if (entity != null) {
				var pictureUrls = JsonSerializer.Deserialize<List<FilePath>>(entity.PicturesUrls).Select(x => x.Url).ToList();
				await _fileMessagesProducerService.SendRemoveImagesMessageAsync(pictureUrls, BucketConstants.ReceiptsImagesBucketName, _entityTypeName);
					}
			return await base.NotBulkDeleteAsync(entityId);
		}

		private async Task<List<Receipt>> DoReceiptsFilteringAsync(ReceiptsFilter receiptsFilter, CancellationToken cancellationToken)
		{
			var tags = new HashSet<Tag>(receiptsFilter.Tags ?? []);
			var data = await _repository.GetAll()
				.Where(r => receiptsFilter.SearchByTitle == string.Empty || r.Title.Value.ToLower().Contains(receiptsFilter.SearchByTitle.ToLower()))
				.Where(r => receiptsFilter.Category == Category.Any || r.Category == receiptsFilter.Category)
				.Where(r => receiptsFilter.CookingDifficulty == CookingDifficulty.Any || r.CookingDifficulty == receiptsFilter.CookingDifficulty)
				.ToListAsync(cancellationToken);
			var filteredByTags = data.Where(r => tags.Count == 0 || Miscellaneous.GetTagsFromString(r.Tags).Any(t => tags.Contains(t))).ToList();
			return filteredByTags;
		}

		public async Task SetPresignedUrlsForReceiptsAsync(List<ShortReceiptOutDTO> receipts, CancellationToken cancellationToken = default)
		{
			using var minioClient = _minioClientFactory.CreateClient();
			foreach (var receipt in receipts)
			{
				var presignedPictures = await MinioUtils.GetPresignedUrlsForFilesFromFilePathsAsync(minioClient, _logger, [new FilePath(receipt.MainPictureUrl)]);
				receipt.MainPictureUrl = presignedPictures[0].Url;
			}
		}

		public async Task SetPresignedUrlForReceiptAsync(FullReceiptOutDTO receipt, CancellationToken cancellationToken = default)
		{
			using var minioClient = _minioClientFactory.CreateClient();
			var presignedPictures = await MinioUtils.GetPresignedUrlsForFilesFromFilePathsAsync(minioClient, _logger, receipt.PicturesUrls);
			receipt.PicturesUrls = presignedPictures;
			receipt.MainPictureUrl = receipt.PicturesUrls[0].Url;
		}
	}
}
