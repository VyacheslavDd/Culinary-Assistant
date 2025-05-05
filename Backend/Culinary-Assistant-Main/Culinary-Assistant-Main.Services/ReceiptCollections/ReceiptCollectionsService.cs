using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.DTO.ReceiptCollection.Interfaces;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.Redis;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.ReceiptsCollections;
using Culinary_Assistant_Main.Services.Users;
using Microsoft.EntityFrameworkCore;
using Minio;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.ReceiptCollections
{
	public class ReceiptCollectionsService(IReceiptCollectionsRepository repository, IElasticReceiptsCollectionsService elasticReceiptCollectionsService,
		IRedisService redisService, IReceiptsService receiptsService, IUsersService usersService, ILogger logger) : 
		BaseService<ReceiptCollection, ReceiptCollectionInModelDTO, ReceiptCollectionUpdateDTO>(repository, logger), IReceiptCollectionsService
	{
		private readonly IElasticReceiptsCollectionsService _elasticReceiptCollectionsService = elasticReceiptCollectionsService;
		private readonly IReceiptCollectionsRepository _receiptCollectionsRepository = repository;
		private readonly IReceiptsService _receiptsService = receiptsService;
		private readonly IRedisService _redisService = redisService;
		private readonly IUsersService _usersService = usersService;

		public async Task<Result<EntitiesResponseWithCountAndPages<ReceiptCollection>>> GetAllByFilterAsync(ReceiptCollectionsFilter filter,
			CancellationToken cancellationToken = default)
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
			foreach (var receiptCollection in receiptCollections)
			{
				await _repository.LoadReferenceAsync(receiptCollection, rc => rc.User);
				await _receiptCollectionsRepository.LoadReceiptsAsync(receiptCollection);
			}
			var entitiesResponse = ApplyPaginationToEntities(receiptCollections, filter);
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
			if (updateRequest.Title != null)
			{
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

		public async Task<Result> AddReceiptsAsyncUsingReceiptCollectionId(Guid receiptCollectionId, List<Guid> receiptIds)
		{
			var existingCollection = await GetByGuidAsync(receiptCollectionId);
			if (existingCollection == null) return Result.Failure("Невозможно добавить рецепты в несуществующую коллекцию");
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

		public async Task<Result> RemoveReceiptsAsync(Guid receiptCollectionId, List<Guid> receiptIds)
		{
			var existingCollection = await GetByGuidAsync(receiptCollectionId);
			if (existingCollection == null) return Result.Failure("Невозможно удалить рецепты из несуществующей коллекции");
			existingCollection.RemoveReceipts(receiptIds);
			await SaveChangesAsync();
			await _redisService.RemoveAsync(RedisUtils.GetCollectionReceiptIdsKey(receiptCollectionId));
			return Result.Success();
		}

		public override async Task<Result<string>> BulkDeleteAsync(Guid entityId)
		{
			await _elasticReceiptCollectionsService.DeleteReceiptCollectionFromIndexAsync(entityId);
			await _redisService.RemoveAsync(RedisUtils.GetCollectionReceiptIdsKey(entityId));
			return await base.BulkDeleteAsync(entityId);
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
	}
}
