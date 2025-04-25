using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Receipts
{
	public interface IElasticReceiptsService
	{
		Task CreateReceiptsIndexAsync();
		Task<Result<List<Guid>>> GetReceiptIdsBySearchParametersAsync(ReceiptsFilterForElasticSearch receiptsFilterForElasticSearch);
		Task IndexReceiptAsync(Receipt receipt, Guid receiptId);
		Task ReindexReceiptAsync(Receipt receipt);
		Task RemoveReceiptIndexAsync(Receipt receipt);
	}
}
