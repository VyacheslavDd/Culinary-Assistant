using Culinary_Assistant.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Filters
{
	public record ReceiptsFilter(int Page = 1, List<Tag>? Tags = null, string SearchByTitle = "", string SearchByIngredients = "", int CookingTimeFrom=0, int CookingTimeTo=1000,
		List<CookingDifficulty>? CookingDifficulties = null, List<Category>? Categories = null, int Limit = 10, Guid? UserId = null,
		SortOption? SortOption = null, bool IsAscendingSorting = true) : IPaginationFilter;
}
