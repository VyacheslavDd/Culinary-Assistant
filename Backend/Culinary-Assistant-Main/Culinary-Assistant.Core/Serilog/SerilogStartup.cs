using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Serilog
{
	public static class SerilogStartup
	{
		public static IHostBuilder AddSerilog(this IHostBuilder host)
		{
			host.UseSerilog((context, config) => config
			.ReadFrom.Configuration(context.Configuration));
			return host;
		}
	}
}
