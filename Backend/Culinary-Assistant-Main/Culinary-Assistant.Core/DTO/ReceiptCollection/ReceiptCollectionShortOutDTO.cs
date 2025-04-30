using Culinary_Assistant.Core.DTO.ReceiptCollection.Interfaces;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Shared.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.ReceiptCollection
{
	public class ReceiptCollectionShortOutDTO : IReceiptCollectionCoversOutDTO, ILikedDTO
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public bool IsPrivate { get; set; }
		public bool IsLiked { get; set; }
		public int Popularity { get; set; }
		public List<FilePath> Covers { get; set; }
		public Guid UserId { get; set; }
	}
}
