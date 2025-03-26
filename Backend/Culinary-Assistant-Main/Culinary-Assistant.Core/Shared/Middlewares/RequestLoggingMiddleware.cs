using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Shared.Middlewares
{
	public class RequestLoggingMiddleware(RequestDelegate next, ILogger logger)
	{
		private readonly RequestDelegate _next = next;
		private readonly ILogger _logger = logger;

		public async Task InvokeAsync(HttpContext httpContext)
		{
			_logger.Information("Выполняется запрос {@method} {@path}", httpContext.Request.Method, httpContext.Request.Path);
			await _next.Invoke(httpContext);
			_logger.Information("Запрос выполнен: {@method} {@path}. Код ответа: {@statusCode}", httpContext.Request.Method, httpContext.Request.Path,
				httpContext.Response.StatusCode);
		}
	}
}
