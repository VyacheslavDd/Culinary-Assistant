using Culinary_Assistant.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Filters
{
	public record ReceiptCollectionsFilter(string Title = "", int Page = 1, int Limit = 15, Guid? UserId = null, CollectionSortOption? SortOption = null,
		bool IsAscendingSorting = true) : IPaginationFilter;
}
