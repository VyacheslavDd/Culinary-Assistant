using Culinary_Assistant.Core.DTO.ReceiptRate;
using Culinary_Assistant_Main.Domain.Fabrics;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.ModelsTests
{
    public class ReceiptRateModel_Tests
	{
		[Test]
		public void CanCreateRate_WithGoodData()
		{
			var userId = Guid.NewGuid();
			var receiptId = Guid.NewGuid();
			var rateInDTO = new RateModelDTO(userId, receiptId, 9);
			var rate = RateFactory.Create<ReceiptRate, Receipt>(rateInDTO);
			Assert.Multiple(() =>
			{
				Assert.That(rate.IsSuccess, Is.True);
				Assert.That(rate.Value.UserId, Is.EqualTo(userId));
				Assert.That(rate.Value.EntityId, Is.EqualTo(receiptId));
				Assert.That(rate.Value.Rating, Is.EqualTo(9));
			});
		}

		[Test]
		public void CannotCreateRate_WithWrongRate()
		{
			var rateInDTO = new RateModelDTO(Guid.NewGuid(), Guid.NewGuid(), 11);
			var rate = RateFactory.Create<ReceiptCollectionRate, ReceiptCollection>(rateInDTO);
			Assert.That(rate.IsFailure, Is.True);
		}
	}
}
