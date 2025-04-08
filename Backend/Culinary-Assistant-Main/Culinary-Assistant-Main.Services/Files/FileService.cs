using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Files
{
	public class FileService(IFileMessagesProducerService fileMessagesProducerService) : IFileService
	{
		private readonly IFileMessagesProducerService _fileMessagesProducerService = fileMessagesProducerService;

		public async Task<List<FilePath>> GenerateFileLinksAndInitiateUploadMessageSending(string bucketName, string entityName, FilesDTO filesDTO)
		{
			var uniqueFileNames = filesDTO.Files.Select(f => FileUtils.GenerateUniqueNameForFileName(f.FileName)).ToList();
			await _fileMessagesProducerService.SendUploadImagesMessagesAsync(filesDTO.Files, uniqueFileNames, bucketName, entityName);
			return uniqueFileNames.Select(fileName => new FilePath($"{bucketName}/{fileName}")).ToList();
		}
	}
}
