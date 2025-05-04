using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Repositories
{
	public class ReceiptRatesRepository(CulinaryAppContext culinaryAppContext) : IReceiptRatesRepository
	{
		private readonly CulinaryAppContext _culinaryAppContext = culinaryAppContext;

		public async Task AddAsync(ReceiptRate receiptRate)
		{
			await _culinaryAppContext.ReceiptRates.AddAsync(receiptRate);
			await SaveChangesAsync();
		}

		public IQueryable<ReceiptRate> GetAllRatesForReceipt(Guid receiptId)
		{
			return _culinaryAppContext.ReceiptRates.Where(rr => rr.ReceiptId == receiptId);
		}

		public async Task<ReceiptRate?> GetAsync(Guid userId, Guid receiptId, CancellationToken cancellationToken = default)
		{
			return await _culinaryAppContext.ReceiptRates.FirstOrDefaultAsync(rr => rr.UserId == userId && rr.ReceiptId == receiptId, cancellationToken);
		}

		public async Task SaveChangesAsync()
		{
			await _culinaryAppContext.SaveChangesAsync();
		}
	}
}
