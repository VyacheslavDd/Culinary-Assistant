using Core.Minio.Service;
using Culinary_Assistant.Core;
using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Minio
{
	public static class MinioStartup
	{
		public static IServiceCollection UseMinioWithoutFileService(this IServiceCollection services, IConfiguration configuration)
		{
			var options = new MinioOptions();
			configuration.Bind(ConfigurationConstants.Minio, options);
			services.AddMinio(config =>
			{
				 config
				.WithSSL(false)
				.WithEndpoint(options.Host)
				.WithCredentials(options.AccessKey, options.SecretKey)
				.WithProxy(new WebProxy(options.Proxy, options.ProxyPort))
				.Build();
			});
			return services;
		}

		public static IServiceCollection UseMinioWithFileService(this IServiceCollection services, IConfiguration configuration)
		{
			services = UseMinioWithoutFileService(services, configuration);
			services.AddScoped<IFileService, FileService>();
			return services;
		}
	}
}
