using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Auth;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.ValueTypes;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Users
{
	public class AuthService(IUsersRepository usersRepository) : IAuthService
	{
		private readonly IUsersRepository _usersRepository = usersRepository;

		public async Task<Result<AuthOutDTO>> RegisterAsync(UserInDTO userInDTO)
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
			await _usersRepository.AddAsync(user);
			//TODO: выдать токены
			return Result.Success(new AuthOutDTO("access", "refresh"));
		}

		public async Task<Result<AuthOutDTO>> AuthenthicateAsync(AuthInDTO authInDTO)
		{
			var isPhone = long.TryParse(authInDTO.Login.Replace("+7", "8"), out long phone);
			var userByLogin = await _usersRepository.GetBySelectorAsync(u => (isPhone && u.Phone.Value == phone) ||
																			  u.Login.Value == authInDTO.Login ||
																			  u.Email.Value == authInDTO.Login);
			if (userByLogin == null)
				return Result.Failure<AuthOutDTO>("Пользователя с таким логином не существует");
			var isVerifiedPassword = BCrypt.Net.BCrypt.Verify(authInDTO.Password, userByLogin.PasswordHash);
			if (!isVerifiedPassword)
				return Result.Failure<AuthOutDTO>("Неправильный пароль");
			if (authInDTO.AdminEntrance && !userByLogin.IsAdmin)
				return Result.Failure<AuthOutDTO>("Данный пользователь не может авторизоваться как админ");
			//TODO: выдать токены; refresh только если rememberMe = true
			return Result.Success(new AuthOutDTO("access", "refresh"));
		}
	}
}
