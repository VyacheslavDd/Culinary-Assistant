using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Services.Files;
using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Culinary_Assistant_Main.Controllers
{
	[Route("api/files")]
	[ApiController]
	public class FilesController(IFileService fileService) : ControllerBase
	{
		private readonly IFileService _fileService = fileService;

		/// <summary>
		/// Загрузить файлы для рецепта и получить ссылки на них
		/// </summary>
		/// <param name="filesDTO">Файлы для загрузки</param>
		/// <returns>Возвращает сгенерированные ссылки на ресурсы</returns>
		/// <response code="200">Результат с успешно загруженными файлами</response>
		/// <response code="500">Ошибка сервера</response>
		[HttpPost]
		[Route("receipts")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UploadReceiptsFilesAsync([FromForm] FilesDTO filesDTO)
		{
			var result = await _fileService.GenerateFileLinksAndInitiateUploadMessageSending(BucketConstants.ReceiptsImagesBucketName, typeof(Receipt).Name, filesDTO);
			return Ok(result);
		}
	}
}
