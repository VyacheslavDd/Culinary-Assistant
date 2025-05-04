using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.ReceiptRate;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.ReceiptRates
{
	public interface IReceiptRateService
	{
		Task<ReceiptRate?> GetAsync(Guid userId, Guid receiptId, CancellationToken cancellationToken = default);
		Task<List<ReceiptRate>> GetAllRatesForReceiptAsync(Guid receiptId, CancellationToken cancellationToken = default);
		Task<Result> AddOrUpdateAsync(ReceiptRateModelDTO receiptRateInDTO);
	}
}
