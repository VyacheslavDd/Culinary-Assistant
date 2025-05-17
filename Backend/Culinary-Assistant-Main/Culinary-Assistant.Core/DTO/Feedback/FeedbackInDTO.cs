using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.Feedback
{
	public record FeedbackInDTO(Guid UserId, Guid ReceiptId, string Text);
}
