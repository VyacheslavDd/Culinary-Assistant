using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.Auth
{
	public record AuthInDTO(string Login, string Password, bool AdminEntrance = false, bool RememberMe = true);
}
