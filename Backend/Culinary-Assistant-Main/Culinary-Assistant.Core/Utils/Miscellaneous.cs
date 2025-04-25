using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Utils
{
	public static class Miscellaneous
	{
		public static string DisplayEntityInfo<T>(T entity, Guid entityId) => $"{entity?.GetType().Name}: {entityId}";
	}
}
