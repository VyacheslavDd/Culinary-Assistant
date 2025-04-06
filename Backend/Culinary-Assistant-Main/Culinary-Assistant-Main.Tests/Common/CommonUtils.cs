using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.Common
{
	public static class CommonUtils
	{
		public static ILogger MockLogger()
		{
			var loggerMock = new Mock<ILogger>();
			loggerMock.Setup(l => l.Information(It.IsAny<string>()));
			return loggerMock.Object;
		}
	}
}
