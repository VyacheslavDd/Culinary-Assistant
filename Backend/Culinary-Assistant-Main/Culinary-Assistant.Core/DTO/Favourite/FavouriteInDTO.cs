using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.Favourite
{
	public record FavouriteInDTO(Guid UserId, Guid ReceiptId);
}
