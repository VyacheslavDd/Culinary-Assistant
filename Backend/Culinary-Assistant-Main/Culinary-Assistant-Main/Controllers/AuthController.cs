using Culinary_Assistant.Core.DTO.Auth;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Infrastructure.Filters;
using Culinary_Assistant_Main.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;

namespace Culinary_Assistant_Main.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController(IAuthService authService, IUsersService usersService, IMinioClientFactory minioClientFactory) : ControllerBase
	{
		private readonly IAuthService _authService = authService;
		private readonly IUsersService _usersService = usersService;
		private readonly IMinioClientFactory _minioClientFactory = minioClientFactory;

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
			var response = await _authService.RegisterAsync(userInDTO, Response);
			if (response.IsSuccess) return Created("auth/register", response.Value.AuthUserOutDTO);
			return BadRequest(response.Error);
		}

		/// <summary>
		/// Аутентифицироваться и авторизоваться на сайте
		/// </summary>
		/// <param name="authInDTO">Логин и пароль. В качестве логина могут использоваться никнейм, почта, телефон</param>
		/// <response code="200">Успешный вход</response>
		/// <response code="400">Некорректные данные</response>
		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> AuthenthicateAsync([FromBody] AuthInDTO authInDTO)
		{
			var response = await _authService.AuthenthicateAsync(authInDTO, Response);
			if (response.IsFailure) return BadRequest(response.Error);
			using var minioClient = _minioClientFactory.CreateClient();
			await _usersService.SetPresignedUrlPictureAsync(minioClient, [response.Value.AuthUserOutDTO]);
			return Ok(response.Value.AuthUserOutDTO);
		}

		/// <summary>
		/// Проверка актуальности сессии, с возможным перевыпуском токенов
		/// </summary>
		/// <response code="200">Успешная проверка</response>
		/// <response code="401">Сессия закончилась и невозможно перевыпустить токены, нужно аутентифицироваться</response>
		[HttpPost]
		[Route("check-in")]
		[ServiceFilter(typeof(AuthenthicationFilter))]
		public IActionResult CheckIn()
		{
			return Ok();
		}
	}
}
