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
		public const int MaxFilesCount = 30;
		public const int ReceiptCollectionMaxCoversCount = 4;
		public const int FilesByMessage = 3;
		public const int FileMaxSize = 16777216;

		public static HashSet<string> SupportedFileExtensions => [".jpg", ".png", ".gif", ".jpeg"];
		public const string ReceiptsElasticSearchIndex = "receipts";
		public const string ReceiptsCollectionsElasticSearchIndex = "receiptscollection";

	}
}
