using Core.Base.Interfaces;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant_Main.Domain.Models;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Receipts
{
	public interface IReceiptsService : IService<Receipt, ReceiptInDTO, UpdateReceiptDTO>
	{
		Task<Result<EntitiesResponseWithCountAndPages<Receipt>>> GetAllAsync(ReceiptsFilter receiptsFilter, CancellationToken cancellationToken = default, List<Guid>? collectionReceiptIds = null);
		Task SetPresignedUrlsForReceiptsAsync(IMinioClient minioClient, List<ShortReceiptOutDTO> receipts, CancellationToken cancellationToken = default);
		Task SetPresignedUrlForReceiptAsync(IMinioClient minioClient, FullReceiptOutDTO receipt, CancellationToken cancellationToken = default);
		Task<Result> SetRatingAsync(Guid receiptId, double rating);
	}
}
