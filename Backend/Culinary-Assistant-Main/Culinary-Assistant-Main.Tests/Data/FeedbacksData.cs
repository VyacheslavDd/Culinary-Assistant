using Culinary_Assistant.Core.DTO.Feedback;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.Data
{
	public static class FeedbacksData
	{
		public static List<Feedback> Feedbacks =>
			[
				Feedback.Create(new FeedbackInDTO(Guid.NewGuid(), Guid.NewGuid(), "hellothere1")).Value,
				Feedback.Create(new FeedbackInDTO(Guid.NewGuid(), Guid.NewGuid(), "hellothere2")).Value,
				Feedback.Create(new FeedbackInDTO(Guid.NewGuid(), Guid.NewGuid(), "hellothere3")).Value,
				Feedback.Create(new FeedbackInDTO(Guid.NewGuid(), Guid.NewGuid(), "hellothere4")).Value,
				Feedback.Create(new FeedbackInDTO(Guid.NewGuid(), Guid.NewGuid(), "hellothere5")).Value,
				Feedback.Create(new FeedbackInDTO(Guid.NewGuid(), Guid.NewGuid(), "hellothere6")).Value
			];
	}
}
