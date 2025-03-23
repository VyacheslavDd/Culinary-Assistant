using Culinary_Assistant.Core.Shared.Serializable;
using Microsoft.AspNetCore.Http;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Minio.Service
{
    public interface IFileService
	{
		Task CreateBucketIfNotExistsAsync(IMinioClient minioClient, string bucketName);
		Task UploadFilesAsync(string bucketName, List<SerializableFormFile> files);
		Task DeleteFilesAsync(string bucketName, List<string> filePaths);
	}
}
