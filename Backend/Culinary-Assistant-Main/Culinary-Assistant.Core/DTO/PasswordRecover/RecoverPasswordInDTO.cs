using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.PasswordRecover
{
	public record RecoverPasswordInDTO(Guid RecoverId, string NewPassword, string NewPasswordConfirmation) : IUpdatePasswordDTO;
}
