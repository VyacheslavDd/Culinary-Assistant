using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Minio.Service
{
    public class FileService(IMinioClientFactory minioClientFactory, ILogger logger) : IFileService
	{
		private readonly IMinioClientFactory _minioClientFactory = minioClientFactory;
		private readonly ILogger _logger = logger;

		public async Task DeleteFilesAsync(string bucketName, List<string> filePaths)
		{
			using var client = _minioClientFactory.CreateClient();
			var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
			var bucketExists = await client.BucketExistsAsync(bucketExistsArgs);
			var fileNames = filePaths.Select(f => f.Split(MiscellaneousConstants.PathSeparator).Last());
			if (!bucketExists) return;
			foreach ( var fileName in fileNames)
			{
				var removeObjectArgs = new RemoveObjectArgs().WithBucket(bucketName).WithObject(fileName);
				await client.RemoveObjectAsync(removeObjectArgs);
				_logger.Information("Файл {@fileName} удален", fileName);
			}
		}

		public async Task CreateBucketIfNotExistsAsync(IMinioClient minioClient, string bucketName)
		{
			var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
			var result = await minioClient.BucketExistsAsync(bucketExistsArgs);
			if (result) return;
			var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
			await minioClient.MakeBucketAsync(makeBucketArgs);
			_logger.Information("Корзина {@bucket} не существовала и была создана.", bucketName);
		}

		public async Task UploadFilesAsync(string bucketName, List<SerializableFormFile> files)
		{
			using var client = _minioClientFactory.CreateClient();
			await CreateBucketIfNotExistsAsync(client, bucketName);
			foreach (var file in files)
			{
				var fileBytesContent = Convert.FromBase64String(file.Content);
				using var stream = new MemoryStream(fileBytesContent);
				stream.Position = 0;
				var putObjectArgs = new PutObjectArgs().WithBucket(bucketName).WithObject(file.FileName)
				.WithContentType(file.ContentType).WithStreamData(stream).WithObjectSize(stream.Length);
				await client.PutObjectAsync(putObjectArgs);
				_logger.Information("Файл {@fileName} загружен.", file.FileName);
			}
		}
	}
}
