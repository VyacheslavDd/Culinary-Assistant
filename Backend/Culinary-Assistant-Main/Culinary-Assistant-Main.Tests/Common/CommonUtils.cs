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
			var minioClientFactoryMock = new Mock<IMinioClientFactory>();
			var usersService = new UsersService(usersRepository, logger, minioClientFactoryMock.Object);
			var rabbitMqOptionsMock = new Mock<IOptions<RabbitMQOptions>>();
			rabbitMqOptionsMock.Setup(o => o.Value).Returns(new RabbitMQOptions() { HostName = "" });
			var producerService = new Mock<IFileMessagesProducerService>();
			producerService.Setup(ps => ps.SendRemoveImagesMessageAsync(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
			var elasticServiceMock = new Mock<IElasticReceiptsService>();
			elasticServiceMock.Setup(esm => esm.GetReceiptIdsBySearchParametersAsync(It.IsAny<ReceiptsFilterForElasticSearch>()))
				.Returns(Task.FromResult(CSharpFunctionalExtensions.Result.Success<List<Guid>>([Guid.Empty])));
			return new ReceiptsService(usersService, producerService.Object, elasticServiceMock.Object, minioClientFactoryMock.Object, receiptsRepository, logger);
		}
	}
}
