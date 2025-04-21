using Culinary_Assistant.Core.Const;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Filters
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true)]
	public class CustomAuthorizeAttribute(params string[] roles) : Attribute, IAuthorizationFilter
	{
		private readonly string[] _roles = roles;

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var user = context.HttpContext.User;
			var userSuitsAnyRole = _roles.Any(user.IsInRole);
			if (!userSuitsAnyRole)
				context.Result = new ForbidResult();
		}
	}
}
