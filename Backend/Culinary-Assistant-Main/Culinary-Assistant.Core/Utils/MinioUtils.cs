using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Shared.Serializable;
using Minio;
using Minio.DataModel.Args;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Utils
{
	public static class MinioUtils
	{
		public static async Task<List<FilePath>> GetPresignedUrlsForFilesFromStringPicturesAsync(IMinioClient minioClient, ILogger logger, string picturesUrls)
		{
			var deserializedUrls = JsonSerializer.Deserialize<List<FilePath>>(picturesUrls);
			return await GetPresignedUrlsForFilesFromFilePathsAsync(minioClient, logger, deserializedUrls);
		}

		public static async Task<List<FilePath>> GetPresignedUrlsForFilesFromFilePathsAsync(IMinioClient minioClient, ILogger logger, List<FilePath> filePaths)
		{
			var presignedUrls = new List<FilePath>();
			foreach (var standardFilePath in filePaths)
			{
				string presignedUrl;
				try
				{
					var bucketAndFileNameData = standardFilePath.Url.Split(MiscellaneousConstants.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
					var args = new PresignedGetObjectArgs()
						.WithBucket(bucketAndFileNameData[0])
						.WithObject(bucketAndFileNameData[1])
						.WithExpiry(BucketConstants.PresignedUrlExpiryTimeInSeconds);
					presignedUrl = await minioClient.PresignedGetObjectAsync(args);
				}
				catch (Exception e)
				{
					logger.Error("Не удалось создать ссылку для файла {@file}: {@exception}", standardFilePath, e.Message);
					presignedUrl = "none";
				}
				presignedUrls.Add(new FilePath(presignedUrl));
			}
			return presignedUrls;
		}
	}
}
