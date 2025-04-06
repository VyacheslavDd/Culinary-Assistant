using Culinary_Assistant.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Filters
{
	public record ReceiptsFilter(int Page = 1, List<Tag>? Tags = null, string SearchByTitle = "", string SearchByIngredients = "",
		CookingDifficulty? CookingDifficulty = CookingDifficulty.Any, Category? Category = Category.Any, int Limit = 10, bool ElasticIngredientsSearch = true);
}
