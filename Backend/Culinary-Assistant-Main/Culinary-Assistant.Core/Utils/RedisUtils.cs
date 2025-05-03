using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Utils
{
	public static class RedisUtils
	{
		public static async Task<T?> GetWithCachingAsync<T>(IRedisService redisService, string key, int cacheTime, Func<Task<T?>> objGetter, CancellationToken cancellationToken)
		{
			var obj = await redisService.GetAsync<T>(key, cancellationToken);
			if (obj.IsSuccess) return obj.Value;
			var dbObj = await objGetter();
			if (dbObj != null) await redisService.SetAsync(key, dbObj, cacheTime);
			return dbObj;
		}

		public static string GetReceiptKey(Guid receiptId) => "receipt_" + receiptId.ToString();

		public static string GetCollectionReceiptIdsKey(Guid collectionId) => "r_collection_ids_" + collectionId.ToString();
	}
}
