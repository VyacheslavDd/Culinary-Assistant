using Core.Base;
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
	public class ReceiptCollectionsRepository(CulinaryAppContext context) : BaseRepository<ReceiptCollection>(context, context.ReceiptCollections),
		IReceiptCollectionsRepository
	{
		public async Task LoadReceiptsAsync(ReceiptCollection receiptCollection)
		{
			await _dbContext.Entry(receiptCollection).Collection(rc => rc.Receipts).Query().OrderByDescending(r => r.UpdatedAt).LoadAsync();
		}
	}
}
