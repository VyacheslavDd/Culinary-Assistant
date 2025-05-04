using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Repositories
{
	public interface IReceiptRatesRepository
	{
		Task<ReceiptRate?> GetAsync(Guid userId, Guid receiptId, CancellationToken cancellationToken = default);
		IQueryable<ReceiptRate> GetAllRatesForReceipt(Guid receiptId);
		Task AddAsync(ReceiptRate receiptRate);
		Task SaveChangesAsync();
	}
}
