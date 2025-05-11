using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Culinary_Assistant.Core.DTO.Feedback;

namespace Culinary_Assistant_Main.Tests.ModelsTests
{
	[TestFixture]
	public class FeedbackModel_Tests
	{
		[TestCase("hellothere")]
		[TestCase("hellothere1")]
		[TestCase("hellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellothere")]
		[TestCase("hellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellothere")]
		public void CanCreateFeedback_WithGoodTextSize(string text)
		{
			var feedBack = Feedback.Create(new FeedbackInDTO(Guid.NewGuid(), Guid.NewGuid(), text));
			Assert.That(feedBack.IsSuccess, Is.True);
		}

		[TestCase("hellother")]
		[TestCase("")]
		[TestCase("hello")]
		[TestCase("hellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellotherehellothere1")]
		public void CannotCreateFeedback_WithBadTextSize(string text)
		{
			var feedBack = Feedback.Create(new FeedbackInDTO(Guid.NewGuid(), Guid.NewGuid(), text));
			Assert.That(feedBack.IsFailure, Is.True);
		}

		[Test]
		public void AllFields_AreFilledIn_AfterCreation()
		{
			var userId = Guid.NewGuid();
			var receiptId = Guid.NewGuid();
			var currentYear = DateTime.UtcNow.Year;
			var feedBack = Feedback.Create(new FeedbackInDTO(userId, receiptId, "hellothere"));
			Assert.Multiple(() =>
			{
				Assert.That(feedBack.IsSuccess, Is.True);
				Assert.That(feedBack.Value.Text.Value, Is.EqualTo("hellothere"));
				Assert.That(feedBack.Value.UpdatedAt.Year, Is.EqualTo(currentYear));
				Assert.That(feedBack.Value.UserId, Is.EqualTo(userId));
				Assert.That(feedBack.Value.ReceiptId, Is.EqualTo(receiptId));
			});
		}
	}
}
