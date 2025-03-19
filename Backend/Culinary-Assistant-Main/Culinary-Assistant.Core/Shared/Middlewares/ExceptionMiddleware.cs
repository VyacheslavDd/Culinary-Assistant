using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Shared.Middlewares
{
	public class ExceptionMiddleware(ILogger logger, RequestDelegate next)
	{
		private readonly RequestDelegate _next = next;
		private readonly ILogger _logger = logger;

		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				await _next.Invoke(httpContext);
			}
			catch (Exception e)
			{
				await HandleException(e, httpContext);
			}
		}

		private async Task HandleException(Exception exception, HttpContext httpContext)
		{
			var problemDetails = new ProblemDetails() { Title = "ERROR!", Detail = exception.Message, Status = StatusCodes.Status500InternalServerError };
			_logger.Error("Произошла ошибка! {@details}", problemDetails);
			_logger.Error("Источник: {@source}", exception.Source);
			_logger.Error("Стек трейс: {@trace}", exception.StackTrace);
			httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
			await httpContext.Response.WriteAsJsonAsync(problemDetails);
		}
	}
}
