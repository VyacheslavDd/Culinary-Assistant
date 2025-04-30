using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Utils
{
	public static class FileUtils
	{
		public static string GenerateUniqueNameForFileName(string fileName)
			=> Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.Ticks + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
	}
}
