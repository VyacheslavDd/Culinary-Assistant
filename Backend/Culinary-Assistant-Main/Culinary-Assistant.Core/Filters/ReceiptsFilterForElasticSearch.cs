using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Filters
{
	public record ReceiptsFilterForElasticSearch(string TitleQuery, string IngredientsQuery, int Page, int Size);
}
