using Culinary_Assistant.Core.Email.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Email
{
	public interface IEmailService
	{
		Task SendPasswordRecoveryEmailAsync(string to, Guid recoverId);
	}
}
