using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Redis
{
	public interface IRedisService
	{
		Task<Result> SetAsync<T>(string key, T value, int expiresInMinutes);
		Task<Result<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default);
		Task RemoveAsync(string key);
	}
}
