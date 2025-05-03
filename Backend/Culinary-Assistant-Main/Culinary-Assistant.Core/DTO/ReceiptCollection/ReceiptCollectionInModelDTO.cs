using Culinary_Assistant.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.ReceiptCollection
{
	public record ReceiptCollectionInModelDTO(string Title, bool IsPrivate, Color Color, Guid UserId, List<Guid>? ReceiptIds);
}
