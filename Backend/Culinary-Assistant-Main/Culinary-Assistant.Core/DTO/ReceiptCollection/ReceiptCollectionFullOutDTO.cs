using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.DTO.ReceiptCollection.Interfaces;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.ReceiptCollection
{
	public class ReceiptCollectionFullOutDTO : IReceiptCollectionCoversOutDTO, IFavouritedDTO
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public bool IsPrivate { get; set; }
		public bool IsFavourited { get; set; }
		public int Popularity { get; set; }
		public List<FilePath> Covers { get; set; }
		public DateTime CreatedAt { get; set; }
		public double Rating { get; set; }
		public Color Color { get; set; }
		public ShortUserOutDTO User { get; set; }
		public List<ShortReceiptOutDTO> Receipts { get; set; }
	}
}
