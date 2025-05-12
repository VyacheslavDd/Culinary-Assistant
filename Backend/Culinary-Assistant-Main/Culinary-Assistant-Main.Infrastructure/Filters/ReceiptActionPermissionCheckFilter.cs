using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Infrastructure.Filters.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Filters
{
	public class ReceiptActionPermissionCheckFilter(IUsersRepository usersRepository, IReceiptsRepository receiptsRepository) : ActionPermissionCheckFilter<Receipt>(usersRepository)
	{
		private readonly IReceiptsRepository _receiptsRepository = receiptsRepository;

		protected override async Task<Receipt?> GetEntityAsync(Guid entityId)
		{
			return await _receiptsRepository.GetBySelectorAsync(r => r.Id == entityId);
		}
	}
}
