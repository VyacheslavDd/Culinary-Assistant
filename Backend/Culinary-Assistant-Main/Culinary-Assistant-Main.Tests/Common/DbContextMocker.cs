using Culinary_Assistant.Core.Options;
using Culinary_Assistant_Main.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.Common
{
	public static class DbContextMocker
	{
		public static CulinaryAppContext CreateInMemoryAppContext()
		{
			var dbContextOptions = new DbContextOptionsBuilder<CulinaryAppContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
			var connectionOptions = new ConnectionOptions() { ConnectionString = "" };
			var connectionOptionsMock = new Mock<IOptions<ConnectionOptions>>();
			connectionOptionsMock.Setup(co => co.Value).Returns(connectionOptions);
			var dbContext = new CulinaryAppContext(dbContextOptions, connectionOptionsMock.Object, isTesting: true);
			return dbContext;
		}
	}
}
