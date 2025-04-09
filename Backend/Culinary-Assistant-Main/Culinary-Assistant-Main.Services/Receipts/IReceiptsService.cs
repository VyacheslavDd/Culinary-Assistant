using Core.Base.Interfaces;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Receipts
{
	public interface IReceiptsService : IService<Receipt, ReceiptInDTO, UpdateReceiptDTO>
	{
		Task<Result<EntitiesResponseWithCountAndPages<Receipt>>> GetAllAsync(ReceiptsFilter receiptsFilter, CancellationToken cancellationToken = default);
		Task SetPresignedUrlsForReceiptsAsync(List<ShortReceiptOutDTO> receipts, CancellationToken cancellationToken = default);
		Task SetPresignedUrlForReceiptAsync(FullReceiptOutDTO receipt, CancellationToken cancellationToken = default);
	}
}
