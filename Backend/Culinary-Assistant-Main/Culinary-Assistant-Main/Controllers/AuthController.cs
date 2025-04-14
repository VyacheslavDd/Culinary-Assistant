using Culinary_Assistant.Core.DTO.Auth;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Culinary_Assistant_Main.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController(IAuthService authService) : ControllerBase
	{
		private readonly IAuthService _authService = authService;

		/// <summary>
		/// Зарегистрироваться на сайте
		/// </summary>
		/// <param name="userInDTO">Логин, email/телефон и пароль</param>
		/// <response code="201">Успешная регистрация</response>
		/// <response code="400">Некорректные или неуникальные данные</response>
		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> RegisterAsync([FromBody] UserInDTO userInDTO)
		{
			var response = await _authService.RegisterAsync(userInDTO);
			if (response.IsSuccess) return Created("auth/register", response.Value);
			return BadRequest(response.Error);
		}

		/// <summary>
		/// Аутентифицироваться и авторизоваться на сайте
		/// </summary>
		/// <param name="authInDTO">Логин и пароль. В качестве логина могут использоваться никнейм, почта, телефон</param>
		/// <response code="200">Успешный вход</response>
		/// <response code="400">Некорректные данные</response>
		[HttpPost]
		[Route("authenthicate")]
		public async Task<IActionResult> AuthenthicateAsync([FromBody] AuthInDTO authInDTO)
		{
			var response = await _authService.AuthenthicateAsync(authInDTO);
			if (response.IsSuccess) return Ok(response.Value);
			return BadRequest(response.Error);
		}
	}
}
