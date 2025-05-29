using Culinary_Assistant.Core.Const;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Http.Service
{
	public class HttpClientService(IHttpClientFactory httpClientFactory, ILogger logger) : IHttpClientService
	{
		private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
		private readonly ILogger _logger = logger;

		public async Task<HttpResponseMessage?> GetAsync(string clientName, string url)
		{
			try
			{
				using var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(MiscellaneousConstants.HttpTimeoutSeconds));
				using var httpClient = _httpClientFactory.CreateClient(clientName);
				var response = await httpClient.GetAsync(url, cancellationToken.Token);
				return response;
			}
			catch (TaskCanceledException)
			{
				_logger.Error("Слишком долгое выполнение запроса {@url}. Операция отменена", url);
				return default;
			}
			catch (HttpRequestException)
			{
				_logger.Error("Запрос {@url} завершился с ошибкой. Повторите позже", url);
				return default;
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				return default;
			}
		}
	}
}
