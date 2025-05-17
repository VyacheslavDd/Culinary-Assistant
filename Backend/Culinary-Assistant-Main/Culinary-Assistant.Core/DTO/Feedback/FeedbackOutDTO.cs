using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.Feedback
{
	public class FeedbackOutDTO
	{
		public Guid Id { get; set; }
		public string Text { get; set; }
		public DateTime UpdatedAt { get; set; }
		public Guid UserId { get; set; }
		public string UserLogin { get; set; }
		public string UserProfilePictureUrl { get; set; }
		public int? Rate { get; set; }
	}
}
