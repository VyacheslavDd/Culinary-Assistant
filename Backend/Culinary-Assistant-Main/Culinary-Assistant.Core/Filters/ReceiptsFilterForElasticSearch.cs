using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Filters
{
	public record ReceiptsFilterForElasticSearch(string TitleQuery, List<string> IngredientsQuery, bool StrictIngredientsSearch, int Page, int Size);
}
