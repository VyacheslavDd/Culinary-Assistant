using AutoMapper;
using Culinary_Assistant.Core.DTO.Like;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Infrastructure.Filters;
using Culinary_Assistant_Main.Services.Likes;
using Culinary_Assistant_Main.Services.Likes.Abstract;
using Culinary_Assistant_Main.Services.ReceiptCollections;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;
using System.Threading;

namespace Culinary_Assistant_Main.Controllers
{
	[Route("api/receipt-collections")]
	[ApiController]
	public class ReceiptCollectionsController(IReceiptCollectionsService receiptCollectionsService, IMinioClientFactory minioClientFactory, ILikesService<ReceiptCollectionLike, ReceiptCollection> collectionLikesService,
		ILikesService<ReceiptLike, Receipt> receiptLikesService, IUsersService usersService, IReceiptsService receiptsService, IMapper mapper) : ControllerBase
	{
		private readonly IReceiptCollectionsService _receiptCollectionsService = receiptCollectionsService;
		private readonly IUsersService _usersService = usersService;
		private readonly IReceiptsService _receiptsService = receiptsService;
		private readonly ILikesService<ReceiptCollectionLike, ReceiptCollection> _collectionLikesService = collectionLikesService;
		private readonly ILikesService<ReceiptLike, Receipt> _receiptLikesService = receiptLikesService;
		private readonly IMinioClientFactory _minioClientFactory = minioClientFactory;
		private readonly IMapper _mapper = mapper;

		/// <summary>
		/// Получить абсолютно все коллекции рецептов (включая приватные)
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <response code="200">Все коллекции</response>
		/// <response code="204">Ответ с 0 коллекциями</response>
		[HttpGet]
		[Route("all")]
		public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
		{
			var collections = await _receiptCollectionsService.GetAllAsync(cancellationToken);
			var mappedCollections = await MapCollectionsAsync(collections);
			return Ok(mappedCollections);
		}

		/// <summary>
		/// Получить все коллекции рецептов по фильтру, не включая приватные (исключение: использование UserId)
		/// </summary>
		/// <param name="receiptCollectionsFilter">Название коллекции, страница, лимит на страницу</param>
		/// <param name="cancellationToken"></param>
		/// <response code="200">Страница с коллекциями</response>
		/// <response code="500">Ошибка поиска по индексам</response>
		[HttpGet]
		[Route("all/by-filter")]
		[ServiceFilter(typeof(EnrichUserFilter))]
		public async Task<IActionResult> GetAllByFilterAsync([FromQuery] ReceiptCollectionsFilter receiptCollectionsFilter, CancellationToken cancellationToken)
		{
			var collections = await _receiptCollectionsService.GetAllByFilterAsync(receiptCollectionsFilter, cancellationToken);
			if (collections.IsFailure) return StatusCode(500, collections.Error);
			if (collections.Value.Data.Count == 0) return Ok(collections.Value);
			var mappedCollections = await MapCollectionsAsync(collections.Value.Data);
			await _collectionLikesService.ApplyLikesInfoForUserAsync(User, mappedCollections);
			return Ok(new EntitiesResponseWithCountAndPages<ReceiptCollectionShortOutDTO>(mappedCollections, collections.Value.EntitiesCount, collections.Value.PagesCount));
		}

		/// <summary>
		/// Получить рецепты коллекции, используя фильтр
		/// </summary>
		/// <param name="id">Guid коллекции</param>
		/// <param name="receiptsFilter">Фильтр рецептов</param>
		/// <param name="cancellationToken"></param>
		/// <response code="200">Получение рецептов коллекции</response>
		/// <response code="400">Некорректные данные</response>
		[HttpGet]
		[Route("{id}/receipts")]
		[ServiceFilter(typeof(EnrichUserFilter))]
		public async Task<IActionResult> GetReceiptsOfCollectionAsync([FromRoute] Guid id, [FromQuery] ReceiptsFilter receiptsFilter, CancellationToken cancellationToken)
		{
			var receiptsIdsRes = await _receiptCollectionsService.GetReceiptIdsAsync(id, cancellationToken);
			if (receiptsIdsRes.IsFailure) return BadRequest(receiptsIdsRes.Error);
			var receiptsRes = await _receiptsService.GetAllAsync(receiptsFilter, cancellationToken, receiptsIdsRes.Value);
			if (receiptsRes.IsFailure) return BadRequest(receiptsIdsRes.Error);
			var mappedReceipts = _mapper.Map<List<ShortReceiptOutDTO>>(receiptsRes.Value.Data);
			using var minioClient = _minioClientFactory.CreateClient();
			await EnrichReceiptsWithDataAsync(mappedReceipts, minioClient, cancellationToken);
			return Ok(new EntitiesResponseWithCountAndPages<ShortReceiptOutDTO>(mappedReceipts, receiptsRes.Value.EntitiesCount, receiptsRes.Value.PagesCount));
		}

		/// <summary>
		/// Получить отдельную коллекцию рецептов
		/// </summary>
		/// <param name="id">Guid коллекции</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <response code="200">Данные о коллекции</response>
		/// <response code="404">Несуществующая коллекция</response>
		[HttpGet]
		[Route("{id}")]
		[ServiceFilter(typeof(EnrichUserFilter))]
		public async Task<IActionResult> GetByGuidAsync([FromRoute] Guid id, CancellationToken cancellationToken)
		{
			var collection = await _receiptCollectionsService.GetByGuidAsync(id, cancellationToken);
			if (collection == null) return NotFound();
			var mappedCollection = _mapper.Map<ReceiptCollectionFullOutDTO>(collection);
			mappedCollection.User = _mapper.Map<ShortUserOutDTO>(collection.User);
			mappedCollection.Receipts = _mapper.Map<List<ShortReceiptOutDTO>>(collection.Receipts);
			_receiptCollectionsService.SetReceiptCovers(collection, mappedCollection);
			using var minioClient = _minioClientFactory.CreateClient();
			await _receiptCollectionsService.SetPresignedUrlsForReceiptCollectionsAsync(minioClient, [mappedCollection]);
			await _usersService.SetPresignedUrlPictureAsync(minioClient, [mappedCollection.User]);
			await _collectionLikesService.ApplyLikeInfoForUserAsync(User, mappedCollection);
			await EnrichReceiptsWithDataAsync(mappedCollection.Receipts, minioClient, cancellationToken);
			return Ok(mappedCollection);
		}

		/// <summary>
		/// Создать коллекцию рецептов
		/// </summary>
		/// <param name="receiptCollectionInModelDTO">Информация о новой коллекции. Можно создать без рецептов или с ними сразу</param>
		/// <response code="201">Успешное создание</response>
		/// <response code="400">Некорректные данные</response>
		[HttpPost]
		[Route("")]
		public async Task<IActionResult> CreateAsync([FromBody] ReceiptCollectionInModelDTO receiptCollectionInModelDTO)
		{
			var res = await _receiptCollectionsService.CreateAsync(receiptCollectionInModelDTO);
			if (res.IsFailure) return BadRequest(res.Error);
			return Created("api/receipt-collections", res.Value);
		}

		/// <summary>
		/// Добавить коллекцию рецептов в избранное
		/// </summary>
		/// <param name="id">Id коллекции рецептов</param>
		/// <response code="204">Успешное добавление в избранное</response>
		/// <response code="400">Некорректные данные или коллекция уже в избранном</response>
		/// <response code="401">Требуется авторизация</response>
		[HttpPost]
		[Route("{id}/favourite")]
		[ServiceFilter(typeof(AuthenthicationFilter))]
		public async Task<IActionResult> FavouriteReceiptCollectionAsync([FromRoute] Guid id)
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(HttpContext.User);
			var res = await _collectionLikesService.AddAsync(new LikeInDTO(userId, id));
			if (res.IsFailure) return BadRequest(res.Error);
			return NoContent();
		}

		/// <summary>
		/// Удалить коллекцию рецептов из избранного
		/// </summary>
		/// <param name="id">Id коллекции рецептов</param>
		/// <response code="204">Успешное удаление из избранного</response>
		/// <response code="400">Некорректные данные</response>
		/// <response code="401">Требуется авторизация</response>
		[HttpDelete]
		[Route("{id}/unfavourite")]
		[ServiceFilter(typeof(AuthenthicationFilter))]
		public async Task<IActionResult> UnfavouriteReceiptAsync([FromRoute] Guid id)
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(HttpContext.User);
			var res = await _collectionLikesService.RemoveAsync(userId, id);
			if (res.IsFailure) return BadRequest(res.Error);
			return NoContent();
		}

		/// <summary>
		/// Обновить коллекцию (без затрагивания рецептов)
		/// </summary>
		/// <param name="id">Guid коллекции</param>
		/// <param name="receiptCollectionUpdateDTO">Новые данные о коллекции</param>
		/// <response code="200">Успешное обновление</response>
		/// <response code="400">Некорректные данные</response>
		[HttpPut]
		[Route("{id}")]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ReceiptCollectionUpdateDTO receiptCollectionUpdateDTO)
		{
			var res = await _receiptCollectionsService.NotBulkUpdateAsync(id, receiptCollectionUpdateDTO);
			if (res.IsFailure) return BadRequest(res.Error);
			return Ok();
		}

		/// <summary>
		/// Добавить рецепты в коллекцию
		/// </summary>
		/// <param name="id">Guid коллекции</param>
		/// <param name="receiptsListDTO">Guid-ы добавляемых рецептов</param>
		/// <response code="200">Успешное добавление рецептов</response>
		/// <response code="400">Некорректные данные</response>
		[HttpPatch]
		[Route("{id}/add-receipts")]
		public async Task<IActionResult> AddReceiptsAsync([FromRoute] Guid id, [FromBody] ReceiptsListDTO receiptsListDTO)
		{
			var res = await _receiptCollectionsService.AddReceiptsAsyncUsingReceiptCollectionId(id, receiptsListDTO.Receipts);
			if (res.IsFailure) return BadRequest(res.Error);
			return Ok();
		}

		/// <summary>
		/// Удалить рецепты из коллекции
		/// </summary>
		/// <param name="id">Guid коллекции</param>
		/// <param name="receiptsListDTO">Guid-ы удаляемых рецептов</param>
		/// <response code="200">Успешное удаление рецептов</response>
		/// <response code="400">Некорректные данные</response>
		[HttpPatch]
		[Route("{id}/remove-receipts")]
		public async Task<IActionResult> RemoveReceiptsAsync([FromRoute] Guid id, [FromBody] ReceiptsListDTO receiptsListDTO)
		{
			var res = await _receiptCollectionsService.RemoveReceiptsAsync(id, receiptsListDTO.Receipts);
			if (res.IsFailure) return BadRequest(res.Error);
			return Ok();
		}

		/// <summary>
		/// Удалить коллекцию рецептов
		/// </summary>
		/// <param name="id">Guid коллекции</param>
		/// <response code="200">Успешное удаление</response>
		/// <response code="400">Некорректные данные</response>
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
		{
			var res = await _receiptCollectionsService.BulkDeleteAsync(id);
			if (res.IsFailure) return BadRequest(res.Error);
			return Ok(res.Value);
		}

		private async Task<List<ReceiptCollectionShortOutDTO>> MapCollectionsAsync(List<ReceiptCollection> receiptCollections)
		{
			var mappedCollections = _mapper.Map<List<ReceiptCollectionShortOutDTO>>(receiptCollections);
			_receiptCollectionsService.SetReceiptNamesWithCovers(receiptCollections, mappedCollections);
			using var minioClient = _minioClientFactory.CreateClient();
			await _receiptCollectionsService.SetPresignedUrlsForReceiptCollectionsAsync(minioClient, mappedCollections);
			return mappedCollections;
		}

		private async Task EnrichReceiptsWithDataAsync(List<ShortReceiptOutDTO> receipts, IMinioClient minioClient, CancellationToken cancellationToken)
		{
			await _receiptsService.SetPresignedUrlsForReceiptsAsync(minioClient, receipts, cancellationToken);
			await _receiptLikesService.ApplyLikesInfoForUserAsync(User, receipts);
		}
	}
}
