﻿using Culinary_Assistant.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Filters
{
	public record ReceiptsFilter(int Page = 1, List<Tag>? Tags = null, string SearchByTitle = "", List<string>? SearchByIngredients = null, bool StrictIngredientsSearch = false,
		int CookingTimeFrom=0, int CookingTimeTo=1000, List<CookingDifficulty>? CookingDifficulties = null, List<Category>? Categories = null, int Limit = 30, Guid? UserId = null,
		ReceiptSortOption? SortOption = null, bool IsAscendingSorting = true) : IPaginationFilter;
}
