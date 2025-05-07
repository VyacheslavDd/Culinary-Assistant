using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Redis
{
	public class RedisService(IDistributedCache distributedCache, ILogger logger) : IRedisService
	{
		private readonly IDistributedCache _distrubutedCache = distributedCache;
		private readonly ILogger _logger = logger;

		private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles };

		public async Task<Result<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default)
		{
			try
			{
				var value = await _distrubutedCache.GetStringAsync(key, cancellationToken);
				if (value == null)
				{
					_logger.Information("Ключ {@key}. Объект отсутствует в кеше!", key);
					return Result.Failure<T>("Объект отсутствует в кеше");
				}
				var deserializedValue = JsonSerializer.Deserialize<T>(value);
				if (deserializedValue == null)
				{
					_logger.Information("Ключ {@key}. Что-то пошло не так при десереализации объекта", key);
					return Result.Failure<T>("Ошибка десериализации");
				}
				return Result.Success(deserializedValue);
			}
			catch (Exception e)
			{
				_logger.Error("Ошибка при получении объекта: {@error}", e.Message);
				return Result.Failure<T>(e.Message);
			}
		}

		public async Task RemoveAsync(string key) => await _distrubutedCache.RemoveAsync(key);

		public async Task<Result> SetAsync<T>(string key, T value, int expiresInMinutes)
		{
			try
			{
				var serializedValue = JsonSerializer.Serialize(value, _jsonSerializerOptions);
				await _distrubutedCache.SetStringAsync(key, serializedValue, new DistributedCacheEntryOptions()
				{
					AbsoluteExpiration = DateTime.UtcNow.AddMinutes(2)
				});
				return Result.Success();
			}
			catch (Exception e)
			{
				_logger.Error("Ошибка при кешировании объекта: {@error}", e.Message);
				return Result.Failure(e.Message);
			}
		}
	}
}
