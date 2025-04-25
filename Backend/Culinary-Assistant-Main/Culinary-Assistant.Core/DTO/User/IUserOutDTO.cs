using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.User
{
	public interface IUserOutDTO
	{
		Guid Id { get; set; }
		string Login { get; set; }
		string? PictureUrl { get; set; }
	}
}
