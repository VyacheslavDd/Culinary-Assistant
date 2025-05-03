using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.Data
{
	public static class ReceiptCollectionsData
	{
		public static List<ReceiptCollection> ReceiptCollections =>
			[
				ReceiptCollection.Create(new ReceiptCollectionInModelDTO("First", false, Color.Red, Guid.NewGuid(), null)).Value,
				ReceiptCollection.Create(new ReceiptCollectionInModelDTO("Second", false, Color.Red, Guid.NewGuid(), null)).Value,
				ReceiptCollection.Create(new ReceiptCollectionInModelDTO("Third", false, Color.Red, Guid.NewGuid(), null)).Value
			];
	}
}
