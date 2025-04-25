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
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.ReceiptCollections
{
	public interface IReceiptCollectionsService : IService<ReceiptCollection, ReceiptCollectionInModelDTO, ReceiptCollectionUpdateDTO>
	{
		Task<Result<EntitiesResponseWithCountAndPages<ReceiptCollection>>> GetAllByFilterAsync(ReceiptCollectionsFilter filter, CancellationToken cancellationToken = default);
		Task SetPresignedUrlsForReceiptCollectionsAsync(IMinioClient minioClient, List<IReceiptCollectionCoversOutDTO> receiptCollections);
		Task<Result> AddReceiptsAsyncUsingReceiptCollectionId(Guid receiptCollectionId, List<Guid> receiptIds);
		Task<Result> RemoveReceiptsAsync(Guid receiptCollectionId, List<Guid> receiptIds);
	}
}
