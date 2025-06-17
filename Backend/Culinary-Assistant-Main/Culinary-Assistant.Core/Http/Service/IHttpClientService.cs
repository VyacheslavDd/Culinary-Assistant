using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Http.Service
{
	public interface IHttpClientService
	{
		Task<HttpResponseMessage?> GetAsync(string clientName, string url);
	}
}
