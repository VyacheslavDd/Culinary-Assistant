using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Utils
{
	public static class TokenUtils
	{
		private static string GenerateToken(Guid userId, List<string> roles, DateTime expiresAt, string secretKey)
		{
			List<Claim> claims = [new Claim("Id", userId.ToString())];
			foreach (var role in roles)
				claims.Add(new Claim(ClaimTypes.Role, role));
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var securityToken = new JwtSecurityToken(claims: claims, expires: expiresAt, signingCredentials: credentials);
			var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
			return token;
		}

		public static TokenOutDTO GenerateAccessAndRefreshTokens(Guid userId, List<string> roles, string secretKey)
		{
			var accessToken = GenerateToken(userId, roles, DateTime.UtcNow.AddMinutes(MiscellaneousConstants.AccessTokenExpirationMinutesTime), secretKey);
			var refreshToken = GenerateToken(userId, roles, DateTime.UtcNow.AddMonths(MiscellaneousConstants.RefreshTokenExpirationMonthsTime), secretKey);
			return new TokenOutDTO(accessToken, refreshToken);
		}

		public static Result<ClaimsPrincipal> ValidateToken(string token, string secretKey)
		{
			try
			{
				var decodedToken = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters()
				{
					AuthenticationType = "Whatever",
					ClockSkew = TimeSpan.Zero,
					ValidateAudience = false,
					ValidateIssuer = false,
					ValidateIssuerSigningKey = true,
					ValidateLifetime = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
				}, out _);
				return Result.Success(decodedToken);
			}
			catch (Exception e)
			{
				return Result.Failure<ClaimsPrincipal>(e.Message);
			}
		}

		public static void SetAccessTokenToCookies(HttpResponse response, string accessToken)
		{
			response.Cookies.Append(MiscellaneousConstants.AccessTokenCookie, accessToken,
				new CookieOptions() { Expires = DateTime.UtcNow.AddMinutes(MiscellaneousConstants.AccessTokenExpirationMinutesTime) });
		}

		public static void SetRefreshTokenToCookies(HttpResponse response, string refreshToken)
		{
			response.Cookies.Append(MiscellaneousConstants.RefreshTokenCookie, refreshToken,
					new CookieOptions() { HttpOnly = true, Expires = DateTime.UtcNow.AddMonths(MiscellaneousConstants.RefreshTokenExpirationMonthsTime) });
		}
	}
}
