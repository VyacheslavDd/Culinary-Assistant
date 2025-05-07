using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.DTO.ReceiptCollection.Interfaces;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.Redis;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.Likes;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.ReceiptsCollections;
using Culinary_Assistant_Main.Services.Users;
using Microsoft.EntityFrameworkCore;
using Minio;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.ReceiptCollections
{
	public class ReceiptCollectionsService(IReceiptCollectionsRepository repository, IElasticReceiptsCollectionsService elasticReceiptCollectionsService,
		IRedisService redisService, ILikesService<ReceiptCollectionLike, ReceiptCollection> collectionsLikesService, IReceiptsService receiptsService, IUsersService usersService, ILogger logger) : 
		BaseService<ReceiptCollection, ReceiptCollectionInModelDTO, ReceiptCollectionUpdateDTO>(repository, logger), IReceiptCollectionsService
	{
		private readonly IElasticReceiptsCollectionsService _elasticReceiptCollectionsService = elasticReceiptCollectionsService;
		private readonly IReceiptCollectionsRepository _receiptCollectionsRepository = repository;
		private readonly ILikesService<ReceiptCollectionLike, ReceiptCollection> _collectionsLikesService = collectionsLikesService;
		private readonly IReceiptsService _receiptsService = receiptsService;
		private readonly IRedisService _redisService = redisService;
		private readonly IUsersService _usersService = usersService;

		private readonly Dictionary<CollectionSortOption?, Func<ReceiptCollection, double>> _orderByExpressions = new()
		{
			{ CollectionSortOption.ByPopularity, (ReceiptCollection receiptCollection) => receiptCollection.Popularity },
			{ CollectionSortOption.ByRating, (ReceiptCollection receiptCollection) => receiptCollection.Rating },
			{ CollectionSortOption.ByDate, (ReceiptCollection receiptCollection) => receiptCollection.CreatedAt.Ticks },
		};

		public async Task<Result<EntitiesResponseWithCountAndPages<ReceiptCollection>>> GetAllByFilterAsync(ReceiptCollectionsFilter filter,
			CancellationToken cancellationToken = default, ClaimsPrincipal? User = null)
		{
			List<Guid> requiredIds = [Guid.Empty];
			if (filter.Title != "")
			{
				var idsResult = await _elasticReceiptCollectionsService.GetReceiptsCollectionsIdsAsync(filter.Title);
				if (idsResult.IsFailure) return Result.Failure<EntitiesResponseWithCountAndPages<ReceiptCollection>>(idsResult.Error);
				requiredIds = idsResult.Value;
			}
			var idsHashset = new HashSet<Guid>(requiredIds);
			var receiptCollections = await _repository
				.GetAll()
				.Where(rc => requiredIds.Count == 1 && requiredIds[0] == Guid.Empty || idsHashset.Contains(rc.Id))
				.Where(rc => filter.UserId == null || rc.UserId == filter.UserId)
				.Where(rc => filter.UserId != null || !rc.IsPrivate)
				.OrderByDescending(rc => rc.UpdatedAt)
				.ToListAsync(cancellationToken);
			if (filter.UserId != null)
			{
				var favouritedCollections = await _collectionsLikesService.GetAllLikedEntitiesForUserAsync(User, cancellationToken);
				if (favouritedCollections.IsSuccess)
				{
					var existingReceiptCollectionIds = new HashSet<Guid>(receiptCollections.Select(r => r.Id));
					foreach (var favouritedCollection in favouritedCollections.Value)
						if (!existingReceiptCollectionIds.Contains(favouritedCollection.Id))
							receiptCollections.Add(favouritedCollection);
				}
				MoveFavouriteReceiptsCollectionToFront(receiptCollections);
			}
			foreach (var receiptCollection in receiptCollections)
			{
				await _repository.LoadReferenceAsync(receiptCollection, rc => rc.User);
				await _receiptCollectionsRepository.LoadReceiptsAsync(receiptCollection);
			}
			var sortedCollections = DoSorting(receiptCollections, filter.SortOption, _orderByExpressions, filter.IsAscendingSorting);
			var entitiesResponse = ApplyPaginationToEntities(sortedCollections, filter);
			return Result.Success(entitiesResponse);
		}

		public override async Task<ReceiptCollection?> GetByGuidAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var receiptCollection = await base.GetByGuidAsync(id, cancellationToken);
			if (receiptCollection != null)
			{
				await _repository.LoadReferenceAsync(receiptCollection, rc => rc.User);
				await _receiptCollectionsRepository.LoadReceiptsAsync(receiptCollection);
			}
			return receiptCollection;
		}

		public async Task<Result<Guid>> CreateWithNameCheckAsync(ReceiptCollectionInModelDTO entityCreateRequest, bool autoSave = true, bool allowFavouriteName = false)
		{
			if (entityCreateRequest.Title == MiscellaneousConstants.FavouriteReceiptsCollectionName && !allowFavouriteName)
				return Result.Failure<Guid>("Данное имя зарезервировано под коллекцию избранных рецептов");
			return await CreateAsync(entityCreateRequest, autoSave);
		}

		public override async Task<Result<Guid>> CreateAsync(ReceiptCollectionInModelDTO entityCreateRequest, bool autoSave = true)
		{
			var user = await _usersService.GetByGuidAsync(entityCreateRequest.UserId);
			if (user == null) return Result.Failure<Guid>("Несуществующий пользователь");
			var createdCollectionResult = ReceiptCollection.Create(entityCreateRequest);
			if (createdCollectionResult.IsFailure) return Result.Failure<Guid>(createdCollectionResult.Error);
			if (entityCreateRequest.ReceiptIds != null)
				await AddReceiptsAsync(createdCollectionResult.Value, entityCreateRequest.ReceiptIds);
			var res = await base.AddToRepositoryAsync(createdCollectionResult);
			if (res.IsSuccess)
				await _elasticReceiptCollectionsService.IndexReceiptCollectionAsync(entityCreateRequest.Title, res.Value);
			return res;
		}

		public override async Task<Result> NotBulkUpdateAsync(Guid entityId, ReceiptCollectionUpdateDTO updateRequest)
		{
			var existingCollection = await _repository.GetBySelectorAsync(rc => rc.Id == entityId);
			if (existingCollection == null) return Result.Failure("Попытка обновить несуществующую коллекцию рецептов");
			if (existingCollection.Title.Value == MiscellaneousConstants.FavouriteReceiptsCollectionName && updateRequest?.IsPrivate == false)
				return Result.Failure("Коллекцию избранных рецептов нельзя сделать публичной!");
			if (updateRequest.Title != null)
			{
				if (updateRequest.Title == MiscellaneousConstants.FavouriteReceiptsCollectionName) return Result.Failure("Данное имя зарезервировано под коллекцию избранных рецептов");
				var setTitleResult = existingCollection.SetTitle(updateRequest.Title);
				if (setTitleResult.IsFailure) return Result.Failure(setTitleResult.Error);
				await _elasticReceiptCollectionsService.ReindexReceiptCollectionAsync(updateRequest.Title, existingCollection.Id);
			}
			existingCollection.SetPrivateState(updateRequest.IsPrivate ?? existingCollection.IsPrivate);
			existingCollection.SetColor(updateRequest.Color ?? existingCollection.Color);
			existingCollection.ActualizeUpdatedAtField();
			if (existingCollection.IsPrivate)
			{
				await _repository.LoadCollectionAsync(existingCollection, rc => rc.Likes);
				existingCollection.ClearLikes();
			}
			return await base.NotBulkUpdateAsync(entityId, updateRequest);
		}

		public async Task<Result> AddReceiptsAsyncUsingReceiptCollectionId(Guid receiptCollectionId, List<Guid> receiptIds, bool allowFavouriteReceiptsCollection = false)
		{
			var existingCollection = await GetByGuidAsync(receiptCollectionId);
			if (existingCollection == null) return Result.Failure("Невозможно добавить рецепты в несуществующую коллекцию");
			if (existingCollection.Title.Value == MiscellaneousConstants.FavouriteReceiptsCollectionName && !allowFavouriteReceiptsCollection)
				return Result.Failure("Нельзя вручную добавлять рецепты в коллекцию избранных рецептов");
			await AddReceiptsAsync(existingCollection, receiptIds);
			await _redisService.RemoveAsync(RedisUtils.GetCollectionReceiptIdsKey(receiptCollectionId));
			return Result.Success();
		}

		private async Task AddReceiptsAsync(ReceiptCollection receiptCollection, List<Guid> receiptIds)
		{
			var newReceiptsIds = new HashSet<Guid>(receiptIds);
			var includedReceiptsIds = new HashSet<Guid>(receiptCollection.Receipts.Select(r => r.Id));
			var receiptsToAddIds = newReceiptsIds.Except(includedReceiptsIds);
			var receiptsToAdd = new List<Receipt>();
			foreach (var receiptId in receiptsToAddIds)
			{
				var receipt = await _receiptsService.GetByGuidAsync(receiptId);
				if (receipt != null) receiptsToAdd.Add(receipt);
			}
			receiptCollection.AddReceipts(receiptsToAdd);
			await SaveChangesAsync();
		}

		public async Task<Result> RemoveReceiptsAsync(Guid receiptCollectionId, List<Guid> receiptIds, bool allowFavouriteReceiptsCollection = false)
		{
			var existingCollection = await GetByGuidAsync(receiptCollectionId);
			if (existingCollection == null) return Result.Failure("Невозможно удалить рецепты из несуществующей коллекции");
			if (existingCollection.Title.Value == MiscellaneousConstants.FavouriteReceiptsCollectionName && !allowFavouriteReceiptsCollection)
				return Result.Failure("Нельзя вручную удалять рецепты из коллекции избранных рецептов");
			existingCollection.RemoveReceipts(receiptIds);
			await SaveChangesAsync();
			await _redisService.RemoveAsync(RedisUtils.GetCollectionReceiptIdsKey(receiptCollectionId));
			return Result.Success();
		}

		public override async Task<Result<string>> BulkDeleteAsync(Guid entityId)
		{
			var isFavouriteReceiptsCollection = await IsFavouriteReceiptsCollection(entityId);
			if (isFavouriteReceiptsCollection) return Result.Failure<string>("Нельзя удалить коллекцию избранных рецептов");
			await _elasticReceiptCollectionsService.DeleteReceiptCollectionFromIndexAsync(entityId);
			await _redisService.RemoveAsync(RedisUtils.GetCollectionReceiptIdsKey(entityId));
			return await base.BulkDeleteAsync(entityId);
		}

		public override async Task<Result<string>> NotBulkDeleteAsync(Guid entityId)
		{
			var isFavouriteReceiptsCollection = await IsFavouriteReceiptsCollection(entityId);
			if (isFavouriteReceiptsCollection) return Result.Failure<string>("Нельзя удалить коллекцию избранных рецептов");
			return await base.NotBulkDeleteAsync(entityId);
		}

		public async Task SetPresignedUrlsForReceiptCollectionsAsync<T>(IMinioClient minioClient, List<T> receiptCollections) where T: IReceiptCollectionCoversOutDTO
		{
			foreach (var rc in receiptCollections)
			{
				var filePaths = await MinioUtils.GetPresignedUrlsForFilesFromFilePathsAsync(minioClient, _logger, rc.Covers);
				rc.Covers = filePaths;
			}
		}

		public void SetReceiptNamesWithCovers(List<ReceiptCollection> originals, List<ReceiptCollectionShortOutDTO> mappedCollections)
		{
			for (var i = 0; i < originals.Count; i++)
			{
				var original = originals[i];
				var mapped = mappedCollections[i];
				mapped.Covers = [];
				mapped.ReceiptNames = [];
				var takeCounter = Math.Min(MiscellaneousConstants.ReceiptCollectionMaxCoversCount, original.Receipts.Count);
				var counter = 0;
				foreach (var receipt in original.Receipts)
				{
					mapped.Covers.Add(new FilePath(receipt.MainPictureUrl));
					mapped.ReceiptNames.Add(receipt.Title.Value);
					counter++;
					if (counter >= takeCounter) break;
				}
			}
		}

		public void SetReceiptCovers(ReceiptCollection original, ReceiptCollectionFullOutDTO mappedCollection)
		{
			mappedCollection.Covers = [];
			var takeCounter = Math.Min(MiscellaneousConstants.ReceiptCollectionMaxCoversCount, original.Receipts.Count);
			var counter = 0;
			foreach (var receipt in original.Receipts)
			{
				mappedCollection.Covers.Add(new FilePath(receipt.MainPictureUrl));
				counter++;
				if (counter >= takeCounter) break;
			}
		}

		public async Task<Result<List<Guid>>> GetReceiptIdsAsync(Guid receiptCollectionId, CancellationToken cancellationToken = default)
		{
			var receiptIdsRes = await _redisService.GetAsync<List<Guid>>(RedisUtils.GetCollectionReceiptIdsKey(receiptCollectionId), cancellationToken);
			if (receiptIdsRes.IsSuccess) return receiptIdsRes;
			var collection = await GetByGuidAsync(receiptCollectionId, cancellationToken);
			if (collection == null) return Result.Failure<List<Guid>>("Указана несуществующая коллекция");
			var receiptIds = collection.Receipts.Select(r => r.Id).ToList();
			await _redisService.SetAsync(RedisUtils.GetCollectionReceiptIdsKey(receiptCollectionId), receiptIds, MiscellaneousConstants.RedisBigCacheTimeMinutes);
			return Result.Success(receiptIds);
		}

		private static void MoveFavouriteReceiptsCollectionToFront(List<ReceiptCollection> receiptCollections)
		{
			var favouriteCollectionIndex = receiptCollections.FindIndex(rc => rc.Title.Value == MiscellaneousConstants.FavouriteReceiptsCollectionName);
			if (favouriteCollectionIndex == -1) return;
			var favourite = receiptCollections[favouriteCollectionIndex];
			for (var i = favouriteCollectionIndex; i > 0; i--)
				receiptCollections[i] = receiptCollections[i - 1];
			receiptCollections[0] = favourite;
		}

		private async Task<bool> IsFavouriteReceiptsCollection(Guid collectionId)
		{
			var collection = await _repository.GetBySelectorAsync(rc => rc.Id == collectionId);
			if (collection == null) return false;
			if (collection.Title.Value == MiscellaneousConstants.FavouriteReceiptsCollectionName) return true;
			return false;
		}

		public async Task AddFavouritedReceiptToFavouriteReceiptsCollectionAsync(Guid receiptId, Guid userId)
		{
			var collectionRes = await CreateOrGetFavouriteReceiptsCollectionGuidAsync(userId);
			if (collectionRes.IsFailure)
			{
				_logger.Error("Не удалось создать коллекцию избранных рецептов для пользователя {@id}: {@error}", userId, collectionRes.Error);
				return;
			}
			var addReceiptRes = await AddReceiptsAsyncUsingReceiptCollectionId(collectionRes.Value, [receiptId], true);
			if (addReceiptRes.IsFailure)
				_logger.Error("Не удалось добавить рецепт в коллекцию избранных рецептов {@id}: {@error}", collectionRes.Value, addReceiptRes.Error);
		}

		public async Task RemoveFavouritedReceiptFromFavouriteReceiptsCollectionAsync(Guid receiptId)
		{
			var collectionGuid = (await _repository.GetBySelectorAsync(rc => rc.Title.Value == MiscellaneousConstants.FavouriteReceiptsCollectionName))?.Id ?? Guid.Empty;
			var removeReceiptRes = await RemoveReceiptsAsync(collectionGuid, [receiptId], true);
			if (removeReceiptRes.IsFailure)
				_logger.Error("Не удалось удалить рецепт из коллекции избранных рецептов {@id}: {@error}", collectionGuid, removeReceiptRes.Error);
		}

		private async Task<Result<Guid>> CreateOrGetFavouriteReceiptsCollectionGuidAsync(Guid userId)
		{
			var collection = await _repository.GetBySelectorAsync(rc => rc.Title.Value == MiscellaneousConstants.FavouriteReceiptsCollectionName);
			if (collection != null) return Result.Success(collection.Id);
			var collectionInDTO = new ReceiptCollectionInModelDTO(MiscellaneousConstants.FavouriteReceiptsCollectionName, true, Color.Yellow, userId, null);
			var res = await CreateAsync(collectionInDTO);
			return res;
		}
	}
}
