using Culinary_Assistant.Core.Shared.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.ReceiptCollection.Interfaces
{
	public interface IReceiptCollectionCoversOutDTO
	{
		List<FilePath> Covers { get; set; }
	}
}
