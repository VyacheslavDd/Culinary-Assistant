using Culinary_Assistant.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Filters
{
	public record ReceiptsFilter(int Page = 1, List<Tag>? Tags = null, string SearchByTitle = "", string SearchByIngredients = "", int CookingTimeFrom=0, int CookingTimeTo=1000,
		CookingDifficulty? CookingDifficulty = null, Category? Category = null, int Limit = 10);
}
