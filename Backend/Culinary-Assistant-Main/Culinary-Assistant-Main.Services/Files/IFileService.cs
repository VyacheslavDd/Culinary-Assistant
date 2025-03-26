using Culinary_Assistant.Core.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Files
{
	public interface IFileService
	{
		Task<List<string>> GenerateFileLinksAndInitiateUploadMessageSending(string bucketName, FilesDTO filesDTO);
	}
}
