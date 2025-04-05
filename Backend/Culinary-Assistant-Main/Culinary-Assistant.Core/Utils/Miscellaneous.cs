using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Enums;
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

		public static List<Result> CreateResultList(int count)
		{
			var results = new List<Result>();
			for (var i = 0; i < count; i++)
				results.Add(Result.Success());
			return results;
		}

		public static Result ResultFailureWithAllFailuresFromResultList(List<Result> results)
		{
			var accumulatedString = string.Join("\n", results.Where(r => r.IsFailure).Select(r => r.Error));
			return Result.Failure(accumulatedString);
		}

		public static List<Tag> GetTagsFromString(string tags)
		{
			return tags.Split(MiscellaneousConstants.ValuesSeparator, StringSplitOptions.RemoveEmptyEntries)
				.Select(t => (Tag)int.Parse(t)).ToList();
		}
	}
}
