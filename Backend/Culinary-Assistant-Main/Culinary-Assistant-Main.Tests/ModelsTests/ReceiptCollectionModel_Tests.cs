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
			var receiptCollectionDTO = new ReceiptCollectionInModelDTO("Коллекция", false, Color.Red, new Guid(), null);
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
		public void AddReceipts_WorksCorrectly()
		{
			var nextReceipt = Receipt.Create(new ReceiptInDTO("Салат", "Вкусный салат", [Tag.Lenten], Category.Dinner, CookingDifficulty.Hard,
					50, [new Ingredient("Огурец", 3, Measure.Piece), new Ingredient("Помидор", 2, Measure.Piece)],
					[new CookingStep(1, "Первый", "Порезать огурец"), new CookingStep(2, "Второй", "Порезать помидор")],
					[new FilePath("4")], default)).Value;
			_receiptCollection.AddReceipts([nextReceipt]);
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
			});
		}

		[TestCase("")]
		[TestCase("TOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOOTOOLONGTOO")]
		public void CannotSet_WrongTitle(string title)
		{
			var titleRes = _receiptCollection.SetTitle(title);
			Assert.That(titleRes.IsFailure, Is.True);
		}
	}
}
