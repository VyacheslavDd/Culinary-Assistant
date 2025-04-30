using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.Utils;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Filters
{
	public class EnrichUserFilter(IConfiguration configuration) : IActionFilter
	{
		private readonly string _secretKey = configuration[ConfigurationConstants.JWTSecretKey]!;

		public void OnActionExecuting(ActionExecutingContext context)
		{
			var accessTokenCookie = context.HttpContext.Request.Cookies[MiscellaneousConstants.AccessTokenCookie];
			if (accessTokenCookie == null) return;
			var validatedTokenResult = TokenUtils.ValidateToken(accessTokenCookie, _secretKey);
			if (validatedTokenResult.IsFailure) return;
			context.HttpContext.User = validatedTokenResult.Value;
		}

		public void OnActionExecuted(ActionExecutedContext context) {}
	}
}
