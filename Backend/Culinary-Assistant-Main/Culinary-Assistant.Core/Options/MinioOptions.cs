using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Options
{
	public class MinioOptions
	{
		public string Host { get; set; }
		public string Proxy { get; set; }
		public int ProxyPort { get; set; }
		public string AccessKey { get; set; }
		public string SecretKey { get; set; }
	}
}
