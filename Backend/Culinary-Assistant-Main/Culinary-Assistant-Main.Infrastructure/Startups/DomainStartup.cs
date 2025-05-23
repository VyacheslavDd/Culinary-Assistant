﻿using Core.Base.Interfaces;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Startups
{
    public static class DomainStartup
	{
		public static IServiceCollection AddDomain(this IServiceCollection services)
		{
			services.AddScoped<ReceiptsRepository>();
			services.AddScoped<IReceiptsRepository>(sp => sp.GetRequiredService<ReceiptsRepository>());
			services.AddScoped<IRepository<Receipt>>(sp => sp.GetRequiredService<ReceiptsRepository>());
			services.AddScoped<IUsersRepository, UsersRepository>();
			services.AddScoped<ReceiptCollectionsRepository>();
			services.AddScoped<IReceiptCollectionsRepository>(sp => sp.GetRequiredService<ReceiptCollectionsRepository>());
			services.AddScoped<IRepository<ReceiptCollection>>(sp => sp.GetRequiredService<ReceiptCollectionsRepository>());
			services.AddScoped<ILikesRepository<ReceiptLike, Receipt>, ReceiptLikesRepository>();
			services.AddScoped<ILikesRepository<ReceiptCollectionLike, ReceiptCollection>, ReceiptCollectionLikesRepository>();
			services.AddScoped<IRatesRepository<ReceiptRate, Receipt>, ReceiptRatesRepository>();
			services.AddScoped<IRatesRepository<ReceiptCollectionRate, ReceiptCollection>, ReceiptCollectionRatesRepository>();
			services.AddScoped<IFeedbacksRepository, FeedbacksRepository>();
			return services;
		}
	}
}
