using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Infrastructure.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Repositories
{
	public class ReceiptLikesRepository(CulinaryAppContext context) : LikesRepository<ReceiptLike, Receipt>(context, context.ReceiptLikes)
	{
	}
}
