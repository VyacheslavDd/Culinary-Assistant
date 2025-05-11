using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.Feedbacks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Filters
{
	public class FeedbackActionPermissionCheckFilter(IFeedbacksRepository feedbacksRepository, IUsersRepository usersRepository) : IAsyncActionFilter
	{
		private readonly IFeedbacksRepository _feedbacksRepository = feedbacksRepository;
		private readonly IUsersRepository _usersRepository = usersRepository;

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(context.HttpContext.User);
			var isAdminUser = context.HttpContext.User.IsInRole("Admin");
			var user = await _usersRepository.GetBySelectorAsync(u => u.Id == userId);
			if (user == null)
			{
				await WriteForbiddenResponseAsync(context);
				return;
			}
			if (isAdminUser)
			{
				await next();
				return;
			}
			var hasFeedbackGuid = Guid.TryParse(context.ActionArguments["Id"]?.ToString(), out var feedbackId);
			if (!hasFeedbackGuid)
			{
				await WriteBadRequestResponseAsync(context);
				return;
			}
			var feedback = await _feedbacksRepository.GetBySelectorAsync(f => f.Id == feedbackId);
			if (feedback == null)
			{
				await WriteBadRequestResponseAsync(context);
				return;
			}
			if (feedback.UserId != userId)
			{
				await WriteForbiddenResponseAsync(context);
				return;
			}
			await next();
		}

		private async Task WriteForbiddenResponseAsync(ActionExecutingContext context)
		{
			context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
			await context.HttpContext.Response.WriteAsync("Недостаточно прав для совершения действия");
		}

		private async Task WriteBadRequestResponseAsync(ActionExecutingContext context)
		{
			context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
			await context.HttpContext.Response.WriteAsync("Некорректный Guid отзыва или несуществующий отзыв");
		}
	}
}
