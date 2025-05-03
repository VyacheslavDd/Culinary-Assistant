using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Redis
{
	public static class Startup
	{
		public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
		{
			var redisOptions = new RedisOptions();
			configuration.Bind(ConfigurationConstants.Redis, redisOptions);
			services.AddStackExchangeRedisCache(s =>
			{
				s.InstanceName = redisOptions.Prefix;
				s.Configuration = redisOptions.Host;
			});
			services.AddScoped<IRedisService, RedisService>();
			return services;
		}
	}
}
