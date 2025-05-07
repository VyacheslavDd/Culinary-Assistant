using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.ReceiptRate;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.ReceiptRates
{
    public interface IRateService<T, TRated> where T: Rate<T, TRated>, new() where TRated: Core.Base.Entity<Guid>
	{
		Task<T?> GetAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
		Task<List<T>> GetAllRatesForEntityAsync(Guid id, CancellationToken cancellationToken = default);
		Task<Result> AddOrUpdateAsync(RateModelDTO receiptRateInDTO);
	}
}
