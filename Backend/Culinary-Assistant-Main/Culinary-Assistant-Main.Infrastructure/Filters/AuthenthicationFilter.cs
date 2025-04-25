using Culinary_Assistant.Core.Const;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Culinary_Assistant.Core.Utils;
using Microsoft.Extensions.Configuration;
using Culinary_Assistant.Core.Constants;
using System.Security.Claims;

namespace Culinary_Assistant_Main.Infrastructure.Filters
{
	public class AuthenthicationFilter(IConfiguration configuration) : IAsyncAuthorizationFilter
	{
		private readonly string _secretKey = configuration[ConfigurationConstants.JWTSecretKey]!;

		private static async Task Write401ResponseAsync(AuthorizationFilterContext context, string message)
		{
			context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
			await context.HttpContext.Response.WriteAsync(message);
		}

		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			var accessToken = context.HttpContext.Request.Cookies[MiscellaneousConstants.AccessTokenCookie];
			if (accessToken == null)
			{
				await Write401ResponseAsync(context, "Отсутствует AccessToken!");
				return;
			}
			var accessTokenValidationResult = TokenUtils.ValidateToken(accessToken, _secretKey);
			if (accessTokenValidationResult.IsSuccess)
			{
				context.HttpContext.User = accessTokenValidationResult.Value;
				return;
			}
			var refreshToken = context.HttpContext.Request.Cookies[MiscellaneousConstants.RefreshTokenCookie];
			if (refreshToken == null)
			{
				await Write401ResponseAsync(context, "Срок действия AccessToken истек!");
				return;
			}
			var refreshTokenValidationResult = TokenUtils.ValidateToken(refreshToken, _secretKey);
			if (refreshTokenValidationResult.IsSuccess)
			{
				context.HttpContext.User = refreshTokenValidationResult.Value;
				RegenerateTokens(context, refreshTokenValidationResult.Value);
				return;
			}
			await Write401ResponseAsync(context, "Срок действия токенов истек!");
		}

		private void RegenerateTokens(AuthorizationFilterContext context, ClaimsPrincipal user)
		{
			var userId = Guid.Parse(user.FindFirstValue("Id")!);
			var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
			var tokens = TokenUtils.GenerateAccessAndRefreshTokens(userId, roles, _secretKey);
			context.HttpContext.Response.Cookies.Append(MiscellaneousConstants.AccessTokenCookie, tokens.AccessToken);
			context.HttpContext.Response.Cookies.Append(MiscellaneousConstants.RefreshTokenCookie, tokens.RefreshToken, new CookieOptions() { HttpOnly = true });
		}
	}
}
