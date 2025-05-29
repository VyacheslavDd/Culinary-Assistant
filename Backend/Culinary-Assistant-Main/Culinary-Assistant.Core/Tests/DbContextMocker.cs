using Culinary_Assistant.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Tests
{
	public static class DbContextMocker
	{
		public static T CreateInMemoryAppContext<T>() where T : DbContext
		{
			var dbContextOptions = new DbContextOptionsBuilder<T>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
			var connectionOptions = new ConnectionOptions() { ConnectionString = "" };
			var connectionOptionsMock = new Mock<IOptions<ConnectionOptions>>();
			connectionOptionsMock.Setup(co => co.Value).Returns(connectionOptions);
			var dbContext = (T)Activator.CreateInstance(typeof(T), dbContextOptions, connectionOptionsMock.Object, true)!;
			return dbContext;
		}
	}
}
