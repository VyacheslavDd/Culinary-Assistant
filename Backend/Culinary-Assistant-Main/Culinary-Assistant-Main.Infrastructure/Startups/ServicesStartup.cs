
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Infrastructure.Filters;
using Culinary_Assistant_Main.Services.Feedbacks;
using Culinary_Assistant_Main.Services.Files;
using Culinary_Assistant_Main.Services.Likes;
using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using Culinary_Assistant_Main.Services.RabbitMQ.ReceiptRates;
using Culinary_Assistant_Main.Services.ReceiptCollections;
using Culinary_Assistant_Main.Services.ReceiptRates;
using Culinary_Assistant_Main.Services.ReceiptRates.Abstract;
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
			services.AddScoped<IRatingMessageProducerService<Receipt>, ReceiptRatingMessageProducerService>();
			services.AddScoped<IRatingMessageProducerService<ReceiptCollection>, CollectionRatingMessageProducerService>();
			services.AddScoped<IUsersService, UsersService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IElasticReceiptsService, ElasticReceiptsService>();
			services.AddScoped<IElasticReceiptsCollectionsService, ElasticReceiptsCollectionsService>();
			services.AddScoped<IReceiptsService, ReceiptsService>();
			services.AddScoped<ISeedService, SeedService>();
			services.AddScoped<ILikesService<ReceiptLike, Receipt>, ReceiptLikesService>();
			services.AddScoped<ILikesService<ReceiptCollectionLike, ReceiptCollection>, ReceiptCollectionLikesService>();
			services.AddScoped<IReceiptCollectionsService, ReceiptCollectionsService>();
			services.AddScoped<IRateService<ReceiptRate, Receipt>, ReceiptRateService>();
			services.AddScoped<IRateService<ReceiptCollectionRate, ReceiptCollection>, CollectionRateService>();
			services.AddScoped<IFeedbacksService, FeedbacksService>();
			services.AddScoped<AuthenthicationFilter>();
			services.AddScoped<EnrichUserFilter>();
			services.AddScoped<CorrelatingUserFilter>();
			services.AddScoped<FeedbackActionPermissionCheckFilter>();
			return services;
		}
	}
}
