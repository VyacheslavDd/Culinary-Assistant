using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Shared.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.ReceiptCollection
{
	public class ReceiptCollectionShortOutDTO
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public List<FilePath> Covers { get; set; }
		public ShortUserOutDTO User { get; set; }
	}
}
