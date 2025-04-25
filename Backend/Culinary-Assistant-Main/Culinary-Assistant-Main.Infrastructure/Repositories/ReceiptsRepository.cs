using Core.Base;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Repositories
{
	public class ReceiptsRepository(CulinaryAppContext culinaryAppContext) : BaseRepository<Receipt>(culinaryAppContext, culinaryAppContext.Receipts), IReceiptsRepository
	{
	}
}
