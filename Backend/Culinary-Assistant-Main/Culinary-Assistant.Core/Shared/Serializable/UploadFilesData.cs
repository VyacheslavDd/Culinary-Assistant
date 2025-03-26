using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Shared.Serializable
{
	public record UploadFilesData(string BucketName, List<SerializableFormFile> SerializableFormFiles);
}
