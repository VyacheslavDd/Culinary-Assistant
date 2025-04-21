using AutoMapper;
using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Minio;

namespace Culinary_Assistant_Main.Controllers
{
	[Route("api/receipts")]
	[ApiController]
	public class ReceiptsController(IReceiptsService receiptsService, IUsersService usersService, IMapper mapper) : ControllerBase
	{
		private readonly IReceiptsService _receiptsService = receiptsService;
		private readonly IUsersService _usersService = usersService;
		private readonly IMapper _mapper = mapper;

		/// <summary>
		/// Получить все рецепты
		/// </summary>
		/// <param name="receiptsFilter">Фильтр рецептов</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Список рецептов</returns>
		/// <response code="200">Список рецептов</response>
		/// <response code="204">Нет рецептов с данным фильтром</response>
		[HttpGet]
		[Route("")]
		public async Task<IActionResult> GetAllAsync([FromQuery] ReceiptsFilter receiptsFilter, CancellationToken cancellationToken)
		{
			var receipts = await _receiptsService.GetAllAsync(receiptsFilter, cancellationToken);
			if (receipts.IsFailure) return StatusCode(500, receipts.Error);
			if (receipts.Value.EntitiesCount == 0) return NoContent();
			var mappedReceipts = _mapper.Map<List<ShortReceiptOutDTO>>(receipts.Value.Data);
			await _receiptsService.SetPresignedUrlsForReceiptsAsync(mappedReceipts, cancellationToken);
			var mappedResponse = new EntitiesResponseWithCountAndPages<ShortReceiptOutDTO>(mappedReceipts, receipts.Value.EntitiesCount, receipts.Value.PagesCount);
			return Ok(mappedResponse);
		}

		/// <summary>
		/// Получить отдельный рецепт полностью
		/// </summary>
		/// <param name="id">Id рецепта</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Данные рецепта</returns>
		/// <response code="200">Успешно полученный рецепт</response>
		/// <response code="404">Рецепт не найден</response>
		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> GetByGuidAsync([FromRoute] Guid id, CancellationToken cancellationToken)
		{
			var receipt = await _receiptsService.GetByGuidAsync(id, cancellationToken);
			if (receipt == null) return NotFound();
			var mappedReceipt = _mapper.Map<FullReceiptOutDTO>(receipt);
			await _receiptsService.SetPresignedUrlForReceiptAsync(mappedReceipt, cancellationToken);
			await _usersService.SetPresignedUrlPictureAsync([mappedReceipt.User]);
			return Ok(mappedReceipt);
		}

		/// <summary>
		/// Создать новый рецепт
		/// </summary>
		/// <param name="receiptInDTO">Данные рецепта</param>
		/// <returns>Guid созданного рецепта</returns>
		/// <response code="201">Успешно созданный рецепт</response>
		/// <response code="400">Некорректные данные</response>
		[HttpPost]
		[Route("")]
		public async Task<IActionResult> AddAsync([FromBody] ReceiptInDTO receiptInDTO)
		{
			var response = await _receiptsService.CreateAsync(receiptInDTO);
			if (response.IsFailure) return BadRequest(response.Error);
			return Created("api/receipts", response.Value);
		}

		/// <summary>
		/// Обновить рецепт
		/// </summary>
		/// <param name="id">Id рецепта</param>
		/// <param name="updateReceiptDTO">Данные для обновления рецепта</param>
		/// <returns>Успешный или неуспешный ответ</returns>
		/// <response code="200">Успешное обновление</response>
		/// <response code="400">Некорректные данные</response>
		[HttpPut]
		[Route("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateReceiptDTO updateReceiptDTO)
		{
			var response = await _receiptsService.NotBulkUpdateAsync(id, updateReceiptDTO);
			if (response.IsFailure) return BadRequest(response.Error);
			return Ok();
		}

		/// <summary>
		/// Удалить рецепт
		/// </summary>
		/// <param name="id">Id рецепта</param>
		/// <returns>Количество удаленных строк</returns>
		/// <response code="200">Успешно удаленный рецепт</response>
		/// <response code="400">Ошибка запроса пользователя</response>
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
		{
			var response = await _receiptsService.NotBulkDeleteAsync(id);
			if (response.IsFailure) return BadRequest(response.Error);
			return Ok(response.Value);
		}
	}
}
