using Core.Base.Interfaces;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.DTO.ReceiptCollection.Interfaces;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant_Main.Domain.Models;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.ReceiptCollections
{
	public interface IReceiptCollectionsService : IService<ReceiptCollection, ReceiptCollectionInModelDTO, ReceiptCollectionUpdateDTO>
	{
		Task<Result<EntitiesResponseWithCountAndPages<ReceiptCollection>>> GetAllByFilterAsync(ReceiptCollectionsFilter filter, CancellationToken cancellationToken = default,
			ClaimsPrincipal? User = null);
		Task SetPresignedUrlsForReceiptCollectionsAsync<T>(IMinioClient minioClient, List<T> receiptCollections) where T: IReceiptCollectionCoversOutDTO;
		Task<Result> AddReceiptsAsyncUsingReceiptCollectionId(Guid receiptCollectionId, List<Guid> receiptIds, bool allowFavouriteReceiptsCollection = false);
		Task<Result> RemoveReceiptsAsync(Guid receiptCollectionId, List<Guid> receiptIds, bool allowFavouriteReceiptsCollection = false);
		Task AddFavouritedReceiptToFavouriteReceiptsCollectionAsync(Guid receiptId, Guid userId);
		Task RemoveFavouritedReceiptFromFavouriteReceiptsCollectionAsync(Guid receiptId);
		Task<Result<Guid>> CreateWithNameCheckAsync(ReceiptCollectionInModelDTO entityCreateRequest, bool autoSave = true, bool allowFavouriteName = false);
		Task<Result<List<Guid>>> GetReceiptIdsAsync(Guid receiptCollectionId, CancellationToken cancellationToken = default);
		void SetReceiptNamesWithCovers(List<ReceiptCollection> originals, List<ReceiptCollectionShortOutDTO> mappedCollections);
		void SetReceiptCovers(ReceiptCollection original, ReceiptCollectionFullOutDTO mappedCollection);
	}
}
