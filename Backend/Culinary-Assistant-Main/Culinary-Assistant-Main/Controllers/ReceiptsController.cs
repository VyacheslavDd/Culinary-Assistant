using AutoMapper;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.DTO.Favourite;
using Culinary_Assistant.Core.DTO.Like;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.DTO.ReceiptRate;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.Redis;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Infrastructure.Filters;
using Culinary_Assistant_Main.Services.Likes;
using Culinary_Assistant_Main.Services.ReceiptRates;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Minio;

namespace Culinary_Assistant_Main.Controllers
{
	[Route("api/receipts")]
	[ApiController]
	public class ReceiptsController(IReceiptsService receiptsService, ILikesService<ReceiptLike, Receipt> likesService, IUsersService usersService,
		IRateService<ReceiptRate, Receipt> receiptRateService, IMapper mapper, IMinioClientFactory minioClientFactory) : ControllerBase
	{
		private readonly IReceiptsService _receiptsService = receiptsService;
		private readonly ILikesService<ReceiptLike, Receipt> _likesService = likesService;
		private readonly IRateService<ReceiptRate, Receipt> _receiptRateService = receiptRateService;
		private readonly IMinioClientFactory _minioClientFactory = minioClientFactory;
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
		[ServiceFilter(typeof(EnrichUserFilter))]
		public async Task<IActionResult> GetAllAsync([FromQuery] ReceiptsFilter receiptsFilter, CancellationToken cancellationToken)
		{
			var receipts = await _receiptsService.GetAllAsync(receiptsFilter, cancellationToken);
			if (receipts.IsFailure) return StatusCode(500, receipts.Error);
			if (receipts.Value.EntitiesCount == 0) return Ok(Array.Empty<ShortReceiptOutDTO>());
			var mappedReceipts = _mapper.Map<List<ShortReceiptOutDTO>>(receipts.Value.Data);
			using var minioClient = _minioClientFactory.CreateClient();
			await _receiptsService.SetPresignedUrlsForReceiptsAsync(minioClient, mappedReceipts, cancellationToken);
			await _likesService.ApplyLikesInfoForUserAsync(User, mappedReceipts);
			var mappedResponse = new EntitiesResponseWithCountAndPages<ShortReceiptOutDTO>(mappedReceipts, receipts.Value.EntitiesCount, receipts.Value.PagesCount);
			return Ok(mappedResponse);
		}

		/// <summary>
		/// Оценить рецепт
		/// </summary>
		/// <param name="id">Guid рецепта</param>
		/// <param name="receiptRateInDTO">Оценка рецепта</param>
		/// <response code="204">Успешное оценивание</response>
		/// <response code="400">Некорректные данные</response>
		[HttpPost]
		[Route("{id}/rate")]
		[ServiceFilter(typeof(AuthenthicationFilter))]
		public async Task<IActionResult> RateReceiptAsync([FromRoute] Guid id, [FromBody] RateInDTO receiptRateInDTO)
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(User);
			var receiptRateModelDTO = new RateModelDTO(userId, id, receiptRateInDTO.Rate);
			var res = await _receiptRateService.AddOrUpdateAsync(receiptRateModelDTO);
			if (res.IsFailure) return BadRequest(res.Error);
			return NoContent();
		}

		/// <summary>
		/// Получить оценку пользователя на рецепт
		/// </summary>
		/// <param name="id">Guid рецепта</param>
		/// <param name="cancellationToken"></param>
		/// <response code="200">Возврат оценки. 0 на ошибочные данные/отсутствие оценки/неавторизованность, иначе действительная оценка</response>
		[HttpGet]
		[Route("{id}/rate")]
		[ServiceFilter(typeof(EnrichUserFilter))]
		public async Task<IActionResult> GetRateAsync([FromRoute] Guid id, CancellationToken cancellationToken)
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(User);
			var rate = await _receiptRateService.GetAsync(userId, id, cancellationToken);
			if (rate == null) return Ok(new RateOutDTO(0));
			return Ok(new RateOutDTO(rate.Rating));
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
		[ServiceFilter(typeof(EnrichUserFilter))]
		public async Task<IActionResult> GetByGuidAsync([FromRoute] Guid id, CancellationToken cancellationToken)
		{
			var receipt = await _receiptsService.GetByGuidAsync(id, cancellationToken);
			if (receipt == null) return NotFound();
			var mappedReceipt = _mapper.Map<FullReceiptOutDTO>(receipt);
			using var minioClient = _minioClientFactory.CreateClient();
			await _receiptsService.SetPresignedUrlForReceiptAsync(minioClient, mappedReceipt, cancellationToken);
			await _likesService.ApplyLikeInfoForUserAsync(User, mappedReceipt);
			await _usersService.SetPresignedUrlPictureAsync(minioClient, [mappedReceipt.User]);
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
		/// Добавить рецепт в избранное
		/// </summary>
		/// <param name="id">Id рецепта</param>
		/// <response code="204">Успешное добавление в избранное</response>
		/// <response code="400">Некорректные данные или рецепт уже в избранном</response>
		/// <response code="401">Требуется авторизация</response>
		[HttpPost]
		[Route("{id}/favourite")]
		[ServiceFilter(typeof(AuthenthicationFilter))]
		public async Task<IActionResult> FavouriteReceiptAsync([FromRoute] Guid id)
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(HttpContext.User);
			var res = await _likesService.AddAsync(new LikeInDTO(userId, id));
			if (res.IsFailure) return BadRequest(res.Error);
			return NoContent();
		}

		/// <summary>
		/// Удалить рецепт из избранного
		/// </summary>
		/// <param name="id">Id рецепта</param>
		/// <response code="204">Успешное удаление из избранного</response>
		/// <response code="400">Некорректные данные</response>
		/// <response code="401">Требуется авторизация</response>
		[HttpDelete]
		[Route("{id}/unfavourite")]
		[ServiceFilter(typeof(AuthenthicationFilter))]
		public async Task<IActionResult> UnfavouriteReceiptAsync([FromRoute] Guid id)
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(HttpContext.User);
			var res = await _likesService.RemoveAsync(userId, id);
			if (res.IsFailure) return BadRequest(res.Error);
			return NoContent();
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
