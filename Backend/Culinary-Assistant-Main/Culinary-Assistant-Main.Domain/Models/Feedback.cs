using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Feedback;
using Culinary_Assistant.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models
{
	public class Feedback : Core.Base.Entity<Guid>
	{
		public Text Text { get; private set; }
		public DateTime UpdatedAt { get; private set; }
		public Guid UserId { get; private set; }
		public Guid ReceiptId { get; private set; }
		public User User { get; private set; }
		public Receipt Receipt { get; private set; }
		
		[NotMapped]
		public int? Rate { get; set; }
		[NotMapped]
		public string UserLogin { get; set; }
		[NotMapped]
		public string UserProfilePictureUrl { get; set; }

		public static Result<Feedback> Create(FeedbackInDTO feedbackInDTO)
		{
			var feedback = new Feedback();
			var textRes = feedback.SetText(feedbackInDTO.Text);
			if (textRes.IsFailure) return Result.Failure<Feedback>(textRes.Error);
			feedback.SetUserId(feedbackInDTO.UserId);
			feedback.SetReceiptId(feedbackInDTO.ReceiptId);
			feedback.UpdateRedactedDate();
			return Result.Success(feedback);
		}

		public Result SetText(string text)
		{
			var textRes = Text.Create(text, maxLength: 200, minLength: 10);
			if (textRes.IsFailure) return Result.Failure(textRes.Error);
			Text = textRes.Value;
			return Result.Success();
		}

		public void SetUserId(Guid userId)
		{
			UserId = userId;
		}

		public void SetReceiptId(Guid receiptId)
		{
			ReceiptId = receiptId;
		}

		public void UpdateRedactedDate()
		{
			UpdatedAt = DateTime.UtcNow;
		}
	}
}
