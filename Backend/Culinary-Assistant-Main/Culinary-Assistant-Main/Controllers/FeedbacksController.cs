using AutoMapper;
using Culinary_Assistant.Core.DTO.Feedback;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Infrastructure.Filters;
using Culinary_Assistant_Main.Services.Feedbacks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Culinary_Assistant_Main.Controllers
{
	[Route("api/feedbacks")]
	[ApiController]
	public class FeedbacksController(IFeedbacksService feedbacksService, IMapper mapper) : ControllerBase
	{
		private readonly IFeedbacksService _feedbacksService = feedbacksService;
		private readonly IMapper _mapper = mapper;

		/// <summary>
		/// Получить все отзывы (со всех рецептов)
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <response code="200">Ответ со всеми отзывами</response>
		[HttpGet]
		[Route("")]
		public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
		{
			var feedbacks = await _feedbacksService.GetAllAsync(cancellationToken);
			var mappedFeedbacks = _mapper.Map<List<FeedbackOutDTO>>(feedbacks);
			return Ok(mappedFeedbacks);
		}

		/// <summary>
		/// Получить отдельный отзыв
		/// </summary>
		/// <param name="id">Guid отзыва</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <response code="200">Ответ с отзывом</response>
		/// <response code="404">Несуществующий отзыв</response>
		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> GetByGuidAsync([FromRoute] Guid id, CancellationToken cancellationToken)
		{
			var feedback = await _feedbacksService.GetByGuidAsync(id, cancellationToken);
			if (feedback == null) return NotFound();
			var mappedFeedback = _mapper.Map<FeedbackOutDTO>(feedback);
			return Ok(mappedFeedback);
		}

		/// <summary>
		/// Отправить отзыв на рецепт
		/// </summary>
		/// <param name="feedbackInDTO">Данные об отзыве</param>
		/// <returns></returns>
		/// <response code="201">Guid созданного отзыва</response>
		/// <response code="400">Некорректные данные</response>
		/// <response code="401">Требуется аутентификация</response>
		[HttpPost]
		[Route("")]
		[ServiceFilter(typeof(AuthenthicationFilter))]
		public async Task<IActionResult> CreateAsync([FromBody] FeedbackInitialInDTO feedbackInDTO)
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(User);
			var actualDTO = new FeedbackInDTO(userId, feedbackInDTO.ReceiptId, feedbackInDTO.Text);
			var res = await _feedbacksService.CreateAsync(actualDTO);
			if (res.IsFailure) return BadRequest(res.Error);
			return Created("api/feedbacks", res.Value);
		}

		/// <summary>
		/// Обновить отзыв
		/// </summary>
		/// <param name="id">Guid отзыва</param>
		/// <param name="feedbackUpdateDTO">Новые данные об отзыве</param>
		/// <returns></returns>
		/// <response code="204">Успешное обновление отзыва</response>
		/// <response code="400">Некорректные данные</response>
		/// <response code="401">Требуется аутентификация</response>
		/// <response code="403">Отсутствие необходимых прав</response>
		[HttpPut]
		[Route("{id}")]
		[ServiceFilter(typeof(AuthenthicationFilter))]
		[ServiceFilter(typeof(FeedbackActionPermissionCheckFilter))]
		public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] FeedbackUpdateDTO feedbackUpdateDTO)
		{
			var res = await _feedbacksService.NotBulkUpdateAsync(id, feedbackUpdateDTO);
			if (res.IsFailure) return BadRequest(res.Error);
			return NoContent();
		}

		/// <summary>
		/// Удалить отзыв
		/// </summary>
		/// <param name="id">Guid отзыва</param>
		/// <returns></returns>
		/// <response code="200">Успешное удаление отзыва</response>
		/// <response code="400">Некорректные данные</response>
		/// <response code="401">Требуется аутентификация</response>
		/// <response code="403">Отсутствие необходимых прав</response>
		[HttpDelete]
		[Route("{id}")]
		[ServiceFilter(typeof(AuthenthicationFilter))]
		[ServiceFilter(typeof(FeedbackActionPermissionCheckFilter))]
		public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
		{
			var res = await _feedbacksService.BulkDeleteAsync(id);
			if (res.IsFailure) return BadRequest(res.Error);
			return Ok(res.Value);
		}
	}
}
