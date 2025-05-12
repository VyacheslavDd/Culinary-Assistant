using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models.Interfaces;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.Feedbacks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Filters.Abstract
{
    public abstract class ActionPermissionCheckFilter<T>(IUsersRepository usersRepository) : IAsyncActionFilter where T: IHasUserId
    {
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
            var hasEntityGuid = Guid.TryParse(context.ActionArguments["Id"]?.ToString(), out var entityId);
            if (!hasEntityGuid)
            {
                await WriteBadRequestResponseAsync(context);
                return;
            }
            var entity = await GetEntityAsync(entityId);
            if (entity == null)
            {
                await WriteBadRequestResponseAsync(context);
                return;
            }
            if (entity.UserId != userId)
            {
                await WriteForbiddenResponseAsync(context);
                return;
            }
            await next();
        }

        protected abstract Task<T?> GetEntityAsync(Guid entityId);

        private async Task WriteForbiddenResponseAsync(ActionExecutingContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.HttpContext.Response.WriteAsync("Недостаточно прав для совершения действия");
        }

        private async Task WriteBadRequestResponseAsync(ActionExecutingContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.HttpContext.Response.WriteAsync("Некорректный Guid сущности или она не существует");
        }
    }
}
