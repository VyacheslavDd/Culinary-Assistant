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

namespace Culinary_Assistant_Main.Services.Receipts
{
	public class ReceiptsService(IUsersService usersService, IReceiptsRepository receiptsRepository, ILogger logger) :
		BaseService<Receipt, ReceiptInDTO, UpdateReceiptDTO>(receiptsRepository, logger), IReceiptsService
	{
		private readonly IUsersService _usersService = usersService;

		public async Task<EntitiesResponseWithCountAndPages<Receipt>> GetAllAsync(ReceiptsFilter receiptsFilter, CancellationToken cancellationToken = default)
		{
			var tags = new HashSet<Tag>(receiptsFilter.Tags ?? []);
			var data = await _repository.GetAll()
				.Where(r => receiptsFilter.SearchByTitle == string.Empty || r.Title.Value.ToLower().Contains(receiptsFilter.SearchByTitle.ToLower()))
				.Where(r => receiptsFilter.Category == Category.Any || r.Category == receiptsFilter.Category)
				.Where(r => receiptsFilter.CookingDifficulty == CookingDifficulty.Any || r.CookingDifficulty == receiptsFilter.CookingDifficulty)
				.ToListAsync(cancellationToken);
			var filteredByTags = data.Where(r => tags.Count == 0 || Miscellaneous.GetTagsFromString(r.Tags).Any(t => tags.Contains(t))).ToList();
			var dataCount = filteredByTags.Count;
			var pagesCount = (int)Math.Ceiling((double)dataCount / receiptsFilter.Limit);
			var currentPage = filteredByTags.Skip(receiptsFilter.Limit * (receiptsFilter.Page - 1))
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
			var results = Miscellaneous.CreateResultList(5);
			var existingReceipt = await GetByGuidAsync(entityId);
			if (existingReceipt == null) return Result.Failure("Сущности для обновления не существует");
			existingReceipt.SetCategory(updateRequest.Category ?? existingReceipt.Category);
			existingReceipt.SetCookingDifficulty(updateRequest.CookingDifficulty ?? existingReceipt.CookingDifficulty);
			if (updateRequest.PicturesUrls != null) existingReceipt.SetPictures(updateRequest.PicturesUrls);
			if (updateRequest.Tags != null) existingReceipt.SetTags(updateRequest.Tags);
			results[0] = existingReceipt.SetTitle(updateRequest.Title ?? existingReceipt.Title.Value);
			results[1] = existingReceipt.SetDescription(updateRequest.Description ?? existingReceipt.Description.Value);
			results[2] = existingReceipt.SetCookingTime(updateRequest.CookingTime ?? existingReceipt.CookingTime);
			if (updateRequest.Ingredients != null)
				results[3] = existingReceipt.SetIngredients(updateRequest.Ingredients);

			if (updateRequest.CookingSteps != null)
				results[4] = existingReceipt.SetCookingSteps(updateRequest.CookingSteps);
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
	}
}
