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
using Culinary_Assistant.Core.Redis;
using Culinary_Assistant.Core.Base.Data;

namespace Culinary_Assistant_Main.Services.Receipts
{
	public class ReceiptsService(IUsersService usersService, IFileMessagesProducerService fileMessagesProducerService, IElasticReceiptsService elasticReceiptsService, 
		IReceiptsRepository receiptsRepository, ILogger logger) :
		BaseService<Receipt, ReceiptInDTO, UpdateReceiptDTO>(receiptsRepository, logger), IReceiptsService
	{
		private readonly IUsersService _usersService = usersService;
		private readonly IFileMessagesProducerService _fileMessagesProducerService = fileMessagesProducerService;
		private readonly IElasticReceiptsService _elasticReceiptsService = elasticReceiptsService;

		private readonly Dictionary<ReceiptSortOption?, Func<Receipt, double>> _orderByExpressions = new()
		{
			{ ReceiptSortOption.ByPopularity, (Receipt receipt) => receipt.Popularity },
			{ ReceiptSortOption.ByCookingTime, (Receipt receipt) => receipt.CookingTime },
			{ ReceiptSortOption.ByCalories, (Receipt receipt) => receipt.Nutrients.Calories },
			{ ReceiptSortOption.ByRating, (Receipt receipt) => receipt.Rating },
		};

		public async Task<Result<EntitiesResponseWithCountAndPages<Receipt>>> GetAllAsync(ReceiptsFilter receiptsFilter, CancellationToken cancellationToken = default, List<Guid>? collectionReceiptsIds = null)
		{
			var elasticFilter = new ReceiptsFilterForElasticSearch(receiptsFilter.SearchByTitle, receiptsFilter.SearchByIngredients ?? [], receiptsFilter.StrictIngredientsSearch,
				receiptsFilter.Page, receiptsFilter.Limit);
			List<Guid> requiredReceiptsIds = [Guid.Empty];
			if (elasticFilter.TitleQuery != "" || receiptsFilter.SearchByIngredients != null)
			{
				var idsResult = await _elasticReceiptsService.GetReceiptIdsBySearchParametersAsync(elasticFilter);
				if (idsResult.IsFailure) return Result.Failure<EntitiesResponseWithCountAndPages<Receipt>>(idsResult.Error);
				requiredReceiptsIds = idsResult.Value;
			}
			var filteredReceipts = await DoReceiptsFilteringAsync(requiredReceiptsIds, collectionReceiptsIds, receiptsFilter, cancellationToken);
			var sortedReceipts = DoSorting(filteredReceipts, receiptsFilter.SortOption, _orderByExpressions, receiptsFilter.IsAscendingSorting);
			var response = ApplyPaginationToEntities(sortedReceipts, receiptsFilter);
			return Result.Success(response);
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
			{
				results[3] = existingReceipt.SetIngredients(updateRequest.Ingredients);
				var nutrientsResult = CalculateNutrients(existingReceipt, updateRequest.Ingredients);
				if (nutrientsResult.IsFailure) return Result.Failure(nutrientsResult.Error);
			}
			if (updateRequest.CookingSteps != null)
				results[4] = existingReceipt.SetCookingSteps(updateRequest.CookingSteps);
			if (updateRequest.PicturesUrls != null)
				results[5] = existingReceipt.SetPictures(updateRequest.PicturesUrls);
			if (!results.All(r => r.IsSuccess)) return Miscellaneous.ResultFailureWithAllFailuresFromResultList(results);
			existingReceipt.ActualizeUpdatedAtField();
			var res = await base.NotBulkUpdateAsync(entityId, updateRequest);
			if (res.IsSuccess)
				await _elasticReceiptsService.ReindexReceiptAsync(existingReceipt);
			return res;
		}

		public override async Task<Result<Guid>> CreateAsync(ReceiptInDTO entityCreateRequest, bool autoSave = true)
		{
			var existingUser = await _usersService.GetByGuidAsync(entityCreateRequest.UserId);
			if (existingUser == null) return Result.Failure<Guid>($"Пользователя с Guid {entityCreateRequest.UserId} не существует");
			var receiptResult = Receipt.Create(entityCreateRequest);
			if (!receiptResult.IsSuccess)
				return Result.Failure<Guid>(receiptResult.Error);
			var nutrientsResult = CalculateNutrients(receiptResult.Value, entityCreateRequest.Ingredients);
			if (!nutrientsResult.IsSuccess)
				return Result.Failure<Guid>(nutrientsResult.Error);
			var res = await AddToRepositoryAsync(receiptResult);
			if (res.IsSuccess)
				await _elasticReceiptsService.IndexReceiptAsync(receiptResult.Value, res.Value);
			return res;
		}

		public override async Task<Result<string>> NotBulkDeleteAsync(Guid entityId)
		{
			var entity = await GetByGuidAsync(entityId);
			if (entity != null) {
				var pictureUrls = JsonSerializer.Deserialize<List<FilePath>>(entity.PicturesUrls).Select(x => x.Url).ToList();
				await _fileMessagesProducerService.SendRemoveImagesMessageAsync(pictureUrls, BucketConstants.ReceiptsImagesBucketName, _entityTypeName);
				await _elasticReceiptsService.RemoveReceiptIndexAsync(entity);
				}
			return await base.NotBulkDeleteAsync(entityId);
		}

		public async Task SetPresignedUrlsForReceiptsAsync(IMinioClient minioClient, List<ShortReceiptOutDTO> receipts, CancellationToken cancellationToken = default)
		{
			foreach (var receipt in receipts)
			{
				var presignedPictures = await MinioUtils.GetPresignedUrlsForFilesFromFilePathsAsync(minioClient, _logger, [new FilePath(receipt.MainPictureUrl)]);
				receipt.MainPictureUrl = presignedPictures[0].Url;
			}
		}

		public async Task SetPresignedUrlForReceiptAsync(IMinioClient minioClient, FullReceiptOutDTO receipt, CancellationToken cancellationToken = default)
		{
			var presignedPictures = await MinioUtils.GetPresignedUrlsForFilesFromFilePathsAsync(minioClient, _logger, receipt.PicturesUrls);
			receipt.PicturesUrls = presignedPictures;
			receipt.MainPictureUrl = receipt.PicturesUrls[0].Url;
		}

		private async Task<List<Receipt>> DoReceiptsFilteringAsync(List<Guid> requiredIds, List<Guid>? collectionReceiptsIds, ReceiptsFilter receiptsFilter, CancellationToken cancellationToken)
		{
			var tags = new HashSet<Tag>(receiptsFilter.Tags ?? []);
			var categories = new HashSet<Category>(receiptsFilter.Categories ?? []);
			var difficulties = new HashSet<CookingDifficulty>(receiptsFilter.CookingDifficulties ?? []);
			var hadEmpty = requiredIds.Count > 0 && requiredIds[0] == Guid.Empty;
			var searchForReceiptsInCollectionHashSet = new HashSet<Guid>(collectionReceiptsIds ?? []);
			var searchForIdsHashSet = new HashSet<Guid>(requiredIds);
			var data = await _repository.GetAll()
				.Where(r => collectionReceiptsIds == null || searchForReceiptsInCollectionHashSet.Contains(r.Id))
				.Where(r => hadEmpty || searchForIdsHashSet.Contains(r.Id))
				.Where(r => receiptsFilter.UserId == null || r.UserId == receiptsFilter.UserId)
				.Where(r => receiptsFilter.Categories == null || categories.Contains(r.Category))
				.Where(r => receiptsFilter.CookingDifficulties == null || difficulties.Contains(r.CookingDifficulty))
				.Where(r => r.CookingTime >= receiptsFilter.CookingTimeFrom && r.CookingTime <= receiptsFilter.CookingTimeTo)
				.OrderByDescending(r => r.UpdatedAt)
				.ToListAsync(cancellationToken);
			var filteredByTags = data.Where(r => tags.Count == 0 || Miscellaneous.GetTagsFromString(r.Tags).Any(t => tags.Contains(t))).ToList();
			if (!hadEmpty && receiptsFilter.StrictIngredientsSearch && receiptsFilter.SearchByIngredients != null)
			{
				var strictedReceipts = filteredByTags.Where(r => JsonSerializer.Deserialize<List<Ingredient>>(r.Ingredients).Count == receiptsFilter.SearchByIngredients.Count).ToList();
				return strictedReceipts;
			}
			return filteredByTags;
		}

		public async Task<Result> SetRatingAsync(Guid receiptId, double rating)
		{
			var receipt = await _repository.GetBySelectorAsync(r => r.Id == receiptId);
			if (receipt == null) return Result.Failure("Попытка обновить рейтинг для несуществующего рецепта");
			receipt.SetRating(rating);
			await _repository.SaveChangesAsync();
			return Result.Success();
		}

		private Result CalculateNutrients(Receipt receipt, List<Ingredient> ingredients)
		{
			List<double> nutrientsData = [0, 0, 0, 0];
			var receiptWeight = 0.0;
			ingredients
				.Where(i => i.Measure == Measure.Gram || i.Measure == Measure.Kilogram || i.Measure == Measure.Liter || i.Measure == Measure.Milliliter)
				.ToList()
				.ForEach(i =>
			{
				var ingredientName = i.Name.ToLower();
				var ingredientFound = IngredientsData.Data.TryGetValue(ingredientName, out var nutrients);
				if (ingredientFound)
				{
					var ingredientWeight = (i.Measure == Measure.Kilogram || i.Measure == Measure.Liter) ? i.NumericValue * 1000 : i.NumericValue;
					receiptWeight += ingredientWeight;
					nutrientsData[0] += ingredientWeight / 100 * nutrients.Calories;
					nutrientsData[1] += ingredientWeight / 100 * nutrients.Proteins;
					nutrientsData[2] += ingredientWeight / 100 * nutrients.Fats;
					nutrientsData[3] += ingredientWeight / 100 * nutrients.Carbohydrates;
				}
			});
			if (receiptWeight > 0)
				nutrientsData = nutrientsData.Select(n => n / receiptWeight * 100).ToList();
			var nutrientsResult = receipt.SetNutrients(nutrientsData[0], nutrientsData[1], nutrientsData[2], nutrientsData[3]);
			return nutrientsResult;
		}
	}
}
