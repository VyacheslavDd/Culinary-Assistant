using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Tests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.ModelsTests
{
	[TestFixture]
	public class ReceiptCollectionModel_Tests
	{
		private ReceiptCollection _receiptCollection;

		[SetUp]
		public void SetUp()
		{
			var receiptCollectionDTO = new ReceiptCollectionInModelDTO("Коллекция", false, new Guid(), null);
			var receipts = ReceiptsData.Receipts;
			_receiptCollection = ReceiptCollection.Create(receiptCollectionDTO).Value;
			_receiptCollection.AddReceipts(receipts);
		}

		[Test]
		public void CanSet_CorrectTitle()
		{
			var titleRes = _receiptCollection.SetTitle("COLLECTION...");
			Assert.Multiple(() =>
			{
				Assert.That(titleRes.IsSuccess, Is.True);
				Assert.That(_receiptCollection.Title.Value, Is.EqualTo("COLLECTION..."));
			});
		}

		[Test]
		public void CanSet_UserId()
		{
			var newGuid = new Guid();
			_receiptCollection.SetUserId(newGuid);
			Assert.That(_receiptCollection.UserId, Is.EqualTo(newGuid));
		}

		[Test]
		public void TogglingIsPrivate_Works()
		{
			_receiptCollection.SetPrivateState(true);
			Assert.That(_receiptCollection.IsPrivate, Is.EqualTo(true));
		}

		[Test]
		public void SetNullCovers_WorksCorrectly()
		{
			_receiptCollection.SetCovers(null);
			Assert.That(_receiptCollection.ReceiptCovers, Is.Empty);
		}

		[Test]
		public void SetEmptyCoversList_WorksCorrectly()
		{
			_receiptCollection.SetCovers([]);
			Assert.That(_receiptCollection.ReceiptCovers, Is.Empty);
		}

		[Test]
		public void CanSet_Covers()
		{
			List<FilePath> covers = [new FilePath("1"), new FilePath("2"), new FilePath("3"), new FilePath("4"), new FilePath("5"), new FilePath("6"), new FilePath("7")];
			_receiptCollection.SetCovers(covers);
			Assert.That(_receiptCollection.ReceiptCovers, Is.EqualTo("[{\"Url\":\"1\"},{\"Url\":\"2\"},{\"Url\":\"3\"},{\"Url\":\"4\"},{\"Url\":\"5\"},{\"Url\":\"6\"}]"));
		}

		[Test]
		public void AddReceipts_WorksCorrectly()
		{
			var nextReceipt = Receipt.Create(new ReceiptInDTO("Салат", "Вкусный салат", [Tag.Lean], Category.Dinner, CookingDifficulty.Hard,
					50, [new Ingredient("Огурец", 3, Measure.Piece), new Ingredient("Помидор", 2, Measure.Piece)],
					[new CookingStep(1, "Первый", "Порезать огурец"), new CookingStep(2, "Второй", "Порезать помидор")],
					[new FilePath("4")], default)).Value;
			_receiptCollection.AddReceipts([nextReceipt]);
			Assert.That(_receiptCollection.ReceiptCovers, Is.EqualTo("[{\"Url\":\"https://placehold.co/600x400\"},{\"Url\":\"https://placehold.co/800x400\"},{\"Url\":\"https://placehold.co/1000x400\"},{\"Url\":\"4\"}]"));
		}

		[Test]
		public void RemoveReceipts_WorksCorrectly()
		{
			List<Guid> ids = [Guid.NewGuid(), Guid.NewGuid()];
			_receiptCollection.Receipts.ElementAt(0).SetReceiptId(ids[0]);
			_receiptCollection.Receipts.ElementAt(2).SetReceiptId(ids[1]);
			_receiptCollection.RemoveReceipts(ids);
			Assert.Multiple(() =>
			{
				Assert.That(_receiptCollection.Receipts, Has.Count.EqualTo(1));
				Assert.That(_receiptCollection.ReceiptCovers, Is.EqualTo("[{\"Url\":\"https://placehold.co/800x400\"}]"));
			});
		}

		[Test]
		public void DeletingCover_WorksCorrectly()
		{
			_receiptCollection.DeleteCoversIfPresented(["https://placehold.co/800x400"]);
			Assert.That(_receiptCollection.ReceiptCovers, Is.EqualTo("[{\"Url\":\"https://placehold.co/600x400\"},{\"Url\":\"https://placehold.co/1000x400\"}]"));
		}

		[Test]
		public void UpdatingCover_WorksCorrectly()
		{
			_receiptCollection.UpdateCoverIfPresented("https://placehold.co/800x400", "49");
			Assert.That(_receiptCollection.ReceiptCovers, Is.EqualTo("[{\"Url\":\"https://placehold.co/600x400\"},{\"Url\":\"49\"},{\"Url\":\"https://placehold.co/1000x400\"}]"));
		}

		[TestCase("")]
		[TestCase("TOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOO")]
		public void CannotSet_WrongTitle(string title)
		{
			var titleRes = _receiptCollection.SetTitle(title);
			Assert.That(titleRes.IsFailure, Is.True);
		}

		[Test]
		public void WontHave_MoreCovers_ThanSpecified()
		{
			_receiptCollection.AddReceipts(ReceiptsData.Receipts);
			var covers = JsonSerializer.Deserialize<List<FilePath>>(_receiptCollection.ReceiptCovers);
			Assert.That(covers, Has.Count.EqualTo(MiscellaneousConstants.ReceiptCollectionMaxCoversCount));
		}
	}
}
