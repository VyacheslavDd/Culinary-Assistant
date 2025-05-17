using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.Seed;
using Culinary_Assistant_Main.Services.Users;
using Microsoft.Extensions.Options;
using Minio;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Culinary_Assistant_Main.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Culinary_Assistant.Core.Constants;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant.Core.Redis;
using CSharpFunctionalExtensions;
using Minio.DataModel.Args;

namespace Culinary_Assistant_Main.Tests.Common
{
	public static class CommonUtils
	{
		public static ILogger MockLogger()
		{
			var loggerMock = new Mock<ILogger>();
			loggerMock.Setup(l => l.Information(It.IsAny<string>(), It.IsAny<object[]>()));
			loggerMock.Setup(l => l.Error(It.IsAny<string>(), It.IsAny<object[]>()));
			return loggerMock.Object;
		}

		public static IConfiguration MockConfiguration()
		{
			var confMock = new Mock<IConfiguration>();
			confMock.Setup(c => c[ConfigurationConstants.JWTSecretKey]).Returns("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
			return confMock.Object;
		}

		public static HttpResponse MockHttpResponse()
		{
			var responseMock = new Mock<HttpResponse>();
			var cookiesMock = new Mock<IResponseCookies>();
			responseMock.Setup(r => r.Cookies).Returns(cookiesMock.Object);
			return responseMock.Object;
		}

		public static IReceiptsService MockReceiptsService(CulinaryAppContext dbContext, IUsersRepository usersRepository, ILogger logger)
		{
			var receiptsRepository = new ReceiptsRepository(dbContext);
			var usersService = new UsersService(usersRepository, logger);
			var rabbitMqOptionsMock = new Mock<IOptions<RabbitMQOptions>>();
			rabbitMqOptionsMock.Setup(o => o.Value).Returns(new RabbitMQOptions() { HostName = "" });
			var producerService = new Mock<IFileMessagesProducerService>();
			producerService.Setup(ps => ps.SendRemoveImagesMessageAsync(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
			var elasticServiceMock = new Mock<IElasticReceiptsService>();
			elasticServiceMock.Setup(esm => esm.GetReceiptIdsBySearchParametersAsync(It.IsAny<ReceiptsFilterForElasticSearch>()))
				.Returns(Task.FromResult(Result.Success<List<Guid>>([Guid.Empty])));
			return new ReceiptsService(usersService, producerService.Object, elasticServiceMock.Object, receiptsRepository, logger);
		}

		public static IUsersService MockUsersService(IUsersRepository usersRepository)
		{
			var logger = MockLogger();
			var usersService = new UsersService(usersRepository, logger);
			return usersService;
		}

		public static IRedisService MockRedisService()
		{
			var redisService = new Mock<IRedisService>();
			SetupRedisServiceGetMethodWithType<Receipt>(redisService);
			return redisService.Object;
		}

		public static void SetupRedisServiceGetMethodWithType<T>(Mock<IRedisService> mock)
		{
			mock.Setup(rs => rs.GetAsync<T>(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Result.Failure<T>("error")));
		}

		public static IMapper MockMapper()
		{
			var mapperMock = new Mock<IMapper>();
			mapperMock.Setup(m => m.Map<AuthUserOutDTO>(It.IsAny<User>())).Returns(new AuthUserOutDTO());
			return mapperMock.Object;
		}

		public static IMinioClientFactory MockMinioClientFactory()
		{
			var minioClientMock = new Mock<IMinioClient>();
			minioClientMock.Setup(m => m.PresignedGetObjectAsync(It.IsAny<PresignedGetObjectArgs>())).Returns(Task.FromResult("none"));
			var minioClientFactory = new Mock<IMinioClientFactory>();
			minioClientFactory.Setup(mcf => mcf.CreateClient()).Returns(minioClientMock.Object);
			return minioClientFactory.Object;
		}

		public static async Task<Guid> GetUserGuidByLoginAsync(CulinaryAppContext context, string login)
		{
			var user = await context.Users.FirstAsync(u => u.Login.Value == login);
			return user.Id;
		}

		public static async Task<Guid> GetReceiptGuidByNameAsync(CulinaryAppContext context, string name)
		{
			var receipt = await context.Receipts.FirstAsync(r => r.Title.Value == name);
			return receipt.Id;
		}

		public static async Task<Guid> GetReceiptCollectionGuidByNameAsync(CulinaryAppContext context, string name)
		{
			var receiptCollection = await context.ReceiptCollections.FirstAsync(r => r.Title.Value == name);
			return receiptCollection.Id;
		}
	}
}
