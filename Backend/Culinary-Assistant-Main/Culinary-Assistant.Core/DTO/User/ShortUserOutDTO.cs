using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.User
{
	public class ShortUserOutDTO : IUserOutDTO
	{
		public Guid Id { get; set; }
		public string Login { get; set; }
		public string? PictureUrl { get; set; }
	}
}
