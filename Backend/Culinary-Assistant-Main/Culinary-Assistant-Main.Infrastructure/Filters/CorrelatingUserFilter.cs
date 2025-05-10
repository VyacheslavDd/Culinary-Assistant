using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Filters
{
	public class CorrelatingUserFilter(IUsersRepository usersRepository) : IAsyncActionFilter
	{
		private readonly IUsersRepository _usersRepository = usersRepository;

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(context.HttpContext.User);
			var res = Guid.TryParse(context.ActionArguments["id"]?.ToString(), out var providedGuid);
			var user = await _usersRepository.GetBySelectorAsync(u => u.Id == userId);
			var isAdminUser = user?.IsAdmin ?? false;
			if (res && user != null && (userId == providedGuid || user.IsAdmin))
			{
				await next();
				return;
			}
			context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
			await context.HttpContext.Response.WriteAsync("Недостаточно прав для совершения действия!");
		}
	}
}
