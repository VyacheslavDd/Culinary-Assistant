using Culinary_Assistant.Core.Http.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Http
{
	public static class HttpStartup
	{
		public static IServiceCollection AddHttpClientWithService(this IServiceCollection services, string clientName, string baseAddress)
		{
			services.AddHttpClient(clientName, c =>
			{
				c.BaseAddress = new Uri(baseAddress);
			});
			services.AddScoped<IHttpClientService, HttpClientService>();
			return services;
		}
	}
}
