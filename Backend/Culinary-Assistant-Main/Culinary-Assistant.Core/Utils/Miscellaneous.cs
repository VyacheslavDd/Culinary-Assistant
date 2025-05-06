using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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

		public static Guid RetrieveUserIdFromHttpContextUser(ClaimsPrincipal user)
		{
			if (user == null) return Guid.Empty;
			var guidClaim = user.FindFirst("Id");
			if (guidClaim == null) return Guid.Empty;
			return Guid.Parse(guidClaim.Value);
		}
	}
}
