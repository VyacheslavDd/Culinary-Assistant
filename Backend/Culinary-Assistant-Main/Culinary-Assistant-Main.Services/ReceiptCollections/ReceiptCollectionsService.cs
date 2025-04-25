using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.DTO.ReceiptCollection.Interfaces;
using Culinary_Assistant.Core.Filters;
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
		IReceiptsService receiptsService, IUsersService usersService, ILogger logger) : 
		BaseService<ReceiptCollection, ReceiptCollectionInModelDTO, ReceiptCollectionUpdateDTO>(repository, logger), IReceiptCollectionsService
	{
		private readonly IElasticReceiptsCollectionsService _elasticReceiptCollectionsService = elasticReceiptCollectionsService;
		private readonly IReceiptsService _receiptsService = receiptsService;
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
				.Where(rc => !rc.IsPrivate)
				.ToListAsync(cancellationToken);
			var entitiesResponse = ApplyPaginationToEntities(receiptCollections, filter);
			return Result.Success(entitiesResponse);
		}

		public override async Task<ReceiptCollection?> GetByGuidAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var receiptCollection = await base.GetByGuidAsync(id, cancellationToken);
			if (receiptCollection != null)
			{
				await _repository.LoadReferenceAsync(receiptCollection, rc => rc.User);
				await _repository.LoadCollectionAsync(receiptCollection, rc => rc.Receipts);
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
			return await base.NotBulkUpdateAsync(entityId, updateRequest);
		}

		public async Task<Result> AddReceiptsAsyncUsingReceiptCollectionId(Guid receiptCollectionId, List<Guid> receiptIds)
		{
			var existingCollection = await GetByGuidAsync(receiptCollectionId);
			if (existingCollection == null) return Result.Failure("Невозможно добавить рецепты в несуществующую коллекцию");
			await AddReceiptsAsync(existingCollection, receiptIds);
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
			return Result.Success();
		}

		public override async Task<Result<string>> BulkDeleteAsync(Guid entityId)
		{
			await _elasticReceiptCollectionsService.DeleteReceiptCollectionFromIndexAsync(entityId);
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
	}
}
