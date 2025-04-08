using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.Shared.Serializable;
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
		Task<List<FilePath>> GenerateFileLinksAndInitiateUploadMessageSending(string bucketName, FilesDTO filesDTO);
	}
}
