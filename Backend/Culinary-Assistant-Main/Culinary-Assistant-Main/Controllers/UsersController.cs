using AutoMapper;
using Culinary_Assistant.Core.DTO.PasswordRecover;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Infrastructure.Filters;
using Culinary_Assistant_Main.Services.Likes;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;

namespace Culinary_Assistant_Main.Controllers
{
	[Route("api/users")]
	[ApiController]
	public class UsersController(IUsersService usersService, IReceiptsService receiptsService, ILikesService<ReceiptLike, Receipt> receiptLikesService,
		 IMapper mapper, IMinioClientFactory minioClientFactory) : ControllerBase
	{
		private readonly IUsersService _usersService = usersService;
		private readonly IReceiptsService _receiptsService = receiptsService;
		private readonly ILikesService<ReceiptLike, Receipt> _receiptLikesService = receiptLikesService;
		private readonly IMapper _mapper = mapper;
		private readonly IMinioClientFactory _minioClientFactory = minioClientFactory;

		/// <summary>
		/// Получить краткую информацию о всех пользователях сайта
		/// </summary>
		/// <response code="200">Успешное выполнение</response>
		[HttpGet]
		[Route("")]
		public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
		{
			var users = await _usersService.GetAllAsync(cancellationToken);
			var mappedUsers = _mapper.Map<List<ShortUserOutDTO>>(users);
			using var minioClient = _minioClientFactory.CreateClient();
			await _usersService.SetPresignedUrlPictureAsync(minioClient, mappedUsers);
			return Ok(mappedUsers);
		}

		/// <summary>
		/// Получить полную информацию о пользователе сайта
		/// </summary>
		/// <param name="id">Guid пользователя</param>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="404">Пользователь не найден</response>
		[HttpGet]
		[Route("{id}/full")]
		public async Task<IActionResult> GetByGuidAsync([FromRoute] Guid id, CancellationToken cancellationToken)
		{
			var user = await _usersService.GetByGuidAsync(id, cancellationToken);
			if (user == null)
				return NotFound();
			var mappedUser = _mapper.Map<FullUserOutDTO>(user);
			mappedUser.Receipts = _mapper.Map<List<ShortReceiptOutDTO>>(user.Receipts);
			using var minioClient = _minioClientFactory.CreateClient();
			await _receiptsService.SetPresignedUrlsForReceiptsAsync(minioClient, mappedUser.Receipts);
			await _usersService.SetPresignedUrlPictureAsync(minioClient, [mappedUser]);
			return Ok(mappedUser);
		}

		/// <summary>
		/// Получить краткую информацию о пользователе сайта
		/// </summary>
		/// <param name="id">Guid пользователя</param>
		/// <param name="cancellationToken"></param>
		/// <response code="200">Успешный возврат данных</response>
		/// <response code="404">Пользователь не найден</response>
		[HttpGet]
		[Route("{id}/short")]
		public async Task<IActionResult> GetByGuidShortAsync([FromRoute] Guid id, CancellationToken cancellationToken)
		{
			var user = await _usersService.GetByGuidAsync(id, cancellationToken);
			if (user == null) return NotFound();
			var mappedUser = _mapper.Map<ShortUserOutDTO>(user);
			using var minioClient = _minioClientFactory.CreateClient();
			await _usersService.SetPresignedUrlPictureAsync(minioClient, [mappedUser]);
			return Ok(mappedUser);
		}

		/// <summary>
		/// Получить все избранные рецепты пользователя
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <response code="200">Ответ со всеми избранными рецептами пользователя</response>
		/// <response code="400">Некорректные данные (Требуется аутентификация)</response>
		[HttpGet]
		[Route("receipts/favourites")]
		[ServiceFilter(typeof(EnrichUserFilter))]
		public async Task<IActionResult> GetFavouriteReceiptsAsync(CancellationToken cancellationToken)
		{
			var favouriteReceipts = await _receiptLikesService.GetAllLikedEntitiesForUserAsync(User, cancellationToken);
			if (favouriteReceipts.IsFailure) return BadRequest(favouriteReceipts.Error);
			var mappedReceipts = _mapper.Map<List<ShortReceiptOutDTO>>(favouriteReceipts.Value);
			using var minioClient = _minioClientFactory.CreateClient();
			await _receiptsService.SetPresignedUrlsForReceiptsAsync(minioClient, mappedReceipts, cancellationToken);
			foreach (var receipt in mappedReceipts) receipt.IsFavourited = true;
			return Ok(mappedReceipts);
		}

		/// <summary>
		/// Обновить информацию о пользователе
		/// </summary>
		/// <param name="id">Guid пользователя</param>
		/// <param name="updateUserDTO">Данные для обновления</param>
		/// <response code="200">Успешное обновление пользователя</response>
		/// <response code="400">Некорректные данные</response>
		[HttpPut]
		[Route("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateUserDTO updateUserDTO)
		{
			var updateResult = await _usersService.NotBulkUpdateAsync(id, updateUserDTO);
			if (updateResult.IsSuccess)
			{
				var updatedUser = await _usersService.GetByGuidAsync(id);
				var mappedUser = _mapper.Map<AuthUserOutDTO>(updatedUser);
				using var minioClient = _minioClientFactory.CreateClient();
				await _usersService.SetPresignedUrlPictureAsync(minioClient, [mappedUser]);
				return Ok(mappedUser);
			}
			return BadRequest(updateResult.Error);
		}

		/// <summary>
		/// Обновить пароль пользователя
		/// </summary>
		/// <param name="id">Guid пользователя</param>
		/// <param name="updatePasswordDTO">Данные для обновления пароля: старый и новый с подтверждением</param>
		/// <response code="200">Успешное обновление пароля</response>
		/// <response code="400">Некорректные данные</response>
		[HttpPatch]
		[Route("{id}")]
		public async Task<IActionResult> UpdatePasswordAsync([FromRoute] Guid id, [FromBody] UpdatePasswordDTO updatePasswordDTO)
		{
			var response = await _usersService.UpdatePasswordAsync(id, updatePasswordDTO);
			if (response.IsSuccess) return Ok();
			return BadRequest(response.Error);
		}

		/// <summary>
		/// Восстановить пароль пользователя
		/// </summary>
		/// <param name="recoverPasswordInDTO"></param>
		/// <response code="200">Успешное восстановление пароля</response>
		/// <response code="400">Некорректные данные или неактуальный запрос/его отсутствие</response>
		[HttpPatch]
		[Route("")]
		public async Task<IActionResult> RecoverPasswordAsync([FromBody] RecoverPasswordInDTO recoverPasswordInDTO)
		{
			var response = await _usersService.RecoverPasswordAsync(recoverPasswordInDTO);
			if (response.IsSuccess) return Ok();
			return BadRequest(response.Error);
		}

		/// <summary>
		/// Удалить пользователя
		/// </summary>
		/// <param name="id">Guid пользователя</param>
		/// <response code="200">Успешное удаление</response>
		/// <response code="400">Некорректные данные</response>
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
		{
			var result = await _usersService.BulkDeleteAsync(id);
			if (result.IsSuccess) return Ok(result.Value);
			return BadRequest(result.Error);
		}
	}
}
