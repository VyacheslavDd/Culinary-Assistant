using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Const
{
	public static class MiscellaneousConstants
	{
		public const char ValuesSeparator = '|';
		public const char PathSeparator = '/';
		public const char RolesSeparator = ',';
		public const int MaxFilesCount = 30;
		public const int ReceiptCollectionMaxCoversCount = 6;
		public const int FilesByMessage = 3;
		public const int FileMaxSize = 16777216;
		public const int RoundRatingToDigits = 1;

		public const int MinReceiptRate = 1;
		public const int MaxReceiptRate = 10;

		public static HashSet<string> SupportedFileExtensions => [".jpg", ".png", ".gif", ".jpeg"];
		public const string ReceiptsElasticSearchIndex = "receipts_v2";
		public const string ReceiptsCollectionsElasticSearchIndex = "receiptscollection_v2";

		public const int AccessTokenExpirationMinutesTime = 15;
		public const int RefreshTokenExpirationMonthsTime = 1;
		public const int RedisGeneralCacheTimeMinutes = 2;
		public const int RedisBigCacheTimeMinutes = 30;

		public const string AccessTokenCookie = "AccessToken";
		public const string RefreshTokenCookie = "RefreshToken";

		public const string FavouriteReceiptsCollectionName = "Избранное";
	}
}
