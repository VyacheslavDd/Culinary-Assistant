using AutoMapper;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.DTO.Auth;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant.Core.ValueTypes;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.Handlers.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Users
{
	public class AuthService(IUsersRepository usersRepository, IConfiguration configuration, IMapper mapper) : IAuthService
	{
		private readonly IUsersRepository _usersRepository = usersRepository;
		private readonly string _secretKey = configuration[ConfigurationConstants.JWTSecretKey]!;
		private readonly BaseLoginHandler _loginHandler = HandlerConstructor.GetLoginHandler();
		private readonly IMapper _mapper = mapper;

		public async Task<Result<AuthOutDTO>> RegisterAsync(UserInDTO userInDTO, HttpResponse httpResponse)
		{
			var userResult = User.Create(userInDTO);
			if (userResult.IsFailure)
				return Result.Failure<AuthOutDTO>(userResult.Error);
			var user = userResult.Value;
			var userByLogin = await _usersRepository.GetBySelectorAsync(u => u.Login.Value == user.Login.Value);
			if (userByLogin != null)
				return Result.Failure<AuthOutDTO>("Пользователь с таким логином уже существует");
			if (user.Email.Value != null)
			{
				var userByEmail = await _usersRepository.GetBySelectorAsync(u => u.Email.Value == user.Email.Value);
				if (userByEmail != null)
					return Result.Failure<AuthOutDTO>("К указанной почте уже есть привязанный аккаунт");
			}
			if (user.Phone.Value != null)
			{
				var userByPhone = await _usersRepository.GetBySelectorAsync(u => u.Phone.Value == user.Phone.Value);
				if (userByPhone != null)
					return Result.Failure<AuthOutDTO>("К указанному номеру телефона уже есть привязанный аккаунт");
			}
			var guid = await _usersRepository.AddAsync(user);
			AddTokensToResponseCookies(httpResponse, guid, ["User"], true);
			var mappedUser = _mapper.Map<AuthUserOutDTO>(user);
			return Result.Success(new AuthOutDTO(mappedUser));
		}

		public async Task<Result<AuthOutDTO>> AuthenthicateAsync(AuthInDTO authInDTO, HttpResponse httpResponse)
		{
			var userByLogin = await _loginHandler.Handle(_usersRepository, authInDTO.Login);
			if (userByLogin == null)
				return Result.Failure<AuthOutDTO>("Пользователя с таким логином не существует");
			var isVerifiedPassword = BCrypt.Net.BCrypt.Verify(authInDTO.Password, userByLogin.PasswordHash);
			if (!isVerifiedPassword)
				return Result.Failure<AuthOutDTO>("Неправильный пароль");
			if (authInDTO.AdminEntrance && !userByLogin.IsAdmin)
				return Result.Failure<AuthOutDTO>("Данный пользователь не может авторизоваться как админ");
			AddTokensToResponseCookies(httpResponse, userByLogin.Id, authInDTO.AdminEntrance ? ["Admin"] : ["User"], authInDTO.RememberMe);
			var mappedUser = _mapper.Map<AuthUserOutDTO>(userByLogin);
			return Result.Success(new AuthOutDTO(mappedUser));
		}

		private void AddTokensToResponseCookies(HttpResponse httpResponse, Guid userId, List<string> roles, bool rememberMe)
		{
			var tokens = TokenUtils.GenerateAccessAndRefreshTokens(userId, roles, _secretKey);
			TokenUtils.SetAccessTokenToCookies(httpResponse, tokens.AccessToken);
			if (rememberMe)
				TokenUtils.SetRefreshTokenToCookies(httpResponse, tokens.RefreshToken);
		}
	}
}
