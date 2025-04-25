using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.ServicesResponses
{
	public record EntitiesResponseWithCountAndPages<T>(List<T> Data, int EntitiesCount, int PagesCount);
}
