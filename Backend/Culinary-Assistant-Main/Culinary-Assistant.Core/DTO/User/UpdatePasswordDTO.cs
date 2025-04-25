using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.User
{
	public record UpdatePasswordDTO(string OldPassword, string NewPassword, string NewPasswordConfirmation);
}
