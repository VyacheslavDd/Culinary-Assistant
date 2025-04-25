using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.ProducerServices;
using Culinary_Assistant.Core.Shared.Serializable;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.RabbitMQ.Images
{
	public class FileMessagesProducerService(IOptions<RabbitMQOptions> rabbitMQOptions, ILogger logger) : IFileMessagesProducerService
	{
		private readonly RabbitMQOptions _rabbitMQOptions = rabbitMQOptions.Value;
		private readonly ILogger _logger = logger;

		public async Task SendRemoveImagesMessageAsync(List<string> imagesUrls, string bucketName, string entityInfo)
		{
			var factory = new ConnectionFactory() { HostName = _rabbitMQOptions.HostName };
			using var connection = await factory.CreateConnectionAsync();
			using var channel = await connection.CreateChannelAsync();
			await channel.ExchangeDeclareAsync(RabbitMQConstants.ImagesExchangeName, ExchangeType.Topic);
			var filesData = new RemoveFilesData(bucketName, imagesUrls);
			var imagesUrlsToString = JsonSerializer.Serialize(filesData);
			var bytesData = Encoding.UTF8.GetBytes(imagesUrlsToString);

			await channel.BasicPublishAsync(RabbitMQConstants.ImagesExchangeName, RabbitMQConstants.RemoveUploadsRoutingKey, bytesData);
			_logger.Information("Опубликовано сообщение на удаление файлов: {@files} для {@entityData}", imagesUrls, entityInfo);
		}

		public async Task SendUploadImagesMessagesAsync(List<IFormFile> files, List<string> correspondingUniqueFileNames, string bucketName, string entityInfo)
		{
			var factory = new ConnectionFactory() { HostName = _rabbitMQOptions.HostName };
			using var connection = await factory.CreateConnectionAsync();
			using var channel = await connection.CreateChannelAsync();
			await channel.ExchangeDeclareAsync(RabbitMQConstants.ImagesExchangeName, ExchangeType.Topic);
			var serializableFormFiles = new List<SerializableFormFile>();
			for (var i = 0; i < files.Count; i++)
			{
				using var stream = new MemoryStream();
				await files[i].CopyToAsync(stream);
				var contentBytes = stream.ToArray();
				var contentBase64String = Convert.ToBase64String(contentBytes);
				serializableFormFiles.Add(new SerializableFormFile(correspondingUniqueFileNames[i], files[i].ContentType, contentBase64String));
			};
			for (int i = 0; i < serializableFormFiles.Count; i += MiscellaneousConstants.FilesByMessage)
			{
				var serializableFilesData = new UploadFilesData(bucketName, serializableFormFiles[i..Math.Min(serializableFormFiles.Count, i + MiscellaneousConstants.FilesByMessage)]);
				var bytesSerializableFormFilesData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(serializableFilesData));
				await channel.BasicPublishAsync(RabbitMQConstants.ImagesExchangeName, RabbitMQConstants.UploadsRoutingKey, bytesSerializableFormFilesData);
				_logger.Information("Опубликовано сообщение на публикацию файлов для {@entityData}", entityInfo);
			}
		}
	}
}
