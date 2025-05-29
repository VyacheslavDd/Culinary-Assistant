using Culinary_Assistant.Core.DTO.PasswordRecover;
using Culinary_Assistant_Notifications_Domain.Models;
using Culinary_Assistant_Notifications_Services.PasswordRecoverService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Culinary_Assistant_Notifications.Controllers
{
	[Route("api/password-recovers")]
	[ApiController]
	public class PasswordRecoversController(IPasswordRecoversService passwordRecoversService) : ControllerBase
	{
		private readonly IPasswordRecoversService _passwordRecoversService = passwordRecoversService;

		/// <summary>
		/// Проверить, что по данному Guid лежит самый свежий запрос на смену пароля и получить email пользователя 
		/// </summary>
		/// <param name="id">Guid recover-запроса</param>
		/// <response code="200">Email пользователя</response>
		/// <response code="204">Пустой ответ, отсутствие записи</response>
		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> CheckAndGetLatestRecoveryRowAsync([FromRoute] Guid id)
		{
			var res = await _passwordRecoversService.CheckLatestRecoverRowForUserByRecoverIdAsync(id);
			if (res.IsSuccess) return Ok(res.Value.UserEmail.Value);
			return BadRequest(res.Error);
		}

		/// <summary>
		/// Запросить смену пароля
		/// </summary>
		/// <param name="passwordRecoverInDTO"></param>
		/// <response code="201">Успешное добавление нового запроса</response>
		/// <response code="400">Некорректные данные или нарушение установленных требований</response>
		[HttpPost]
		[Route("")]
		public async Task<IActionResult> AddNewPasswordRecoverRequestAsync([FromBody] PasswordRecoverInDTO passwordRecoverInDTO)
		{
			var res = await _passwordRecoversService.AddAsync(passwordRecoverInDTO);
			if (res.IsFailure) return BadRequest(res.Error);
			return Created();
		}
	}
}
