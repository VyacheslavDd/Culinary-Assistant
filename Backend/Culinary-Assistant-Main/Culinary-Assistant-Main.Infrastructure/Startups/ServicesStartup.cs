using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Startups
{
	public static class ServicesStartup
	{
		public static IServiceCollection AddCustomServices(this IServiceCollection services)
		{
			services.AddScoped<IImageMessagesProducerService, ImageMessagesProducerService>();
			return services;
		}
	}
}
