using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Files
{
	public class FileService(IFileMessagesProducerService fileMessagesProducerService, IConfiguration configuration) : IFileService
	{
		private readonly IFileMessagesProducerService _fileMessagesProducerService = fileMessagesProducerService;
		private readonly string _minioLocalHost = configuration[ConfigurationConstants.MinioLocalHost]!;

		public async Task<List<string>> GenerateFileLinksAndInitiateUploadMessageSending(string bucketName, FilesDTO filesDTO)
		{
			var uniqueFileNames = filesDTO.Files.Select(f => FileUtils.GenerateUniqueNameForFileName(f.FileName)).ToList();
			await _fileMessagesProducerService.SendUploadImagesMessagesAsync(filesDTO.Files, uniqueFileNames, bucketName, filesDTO.EntityInfo);
			return uniqueFileNames.Select(fileName => $"{_minioLocalHost}/{bucketName}/{fileName}").ToList();
		}
	}
}
