
using Culinary_Assistant_Main.Services.Files;
using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.ReceiptsCollections;
using Culinary_Assistant_Main.Services.Seed;
using Culinary_Assistant_Main.Services.Users;
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
			services.AddScoped<IFileService, FileService>();
			services.AddScoped<IFileMessagesProducerService, FileMessagesProducerService>();
			services.AddScoped<IUsersService, UsersService>();
			services.AddScoped<IElasticReceiptsService, ElasticReceiptsService>();
			services.AddScoped<IElasticReceiptsCollectionsService, ElasticReceiptsCollectionsService>();
			services.AddScoped<IReceiptsService, ReceiptsService>();
			services.AddScoped<ISeedService, SeedService>();
			return services;
		}
	}
}
