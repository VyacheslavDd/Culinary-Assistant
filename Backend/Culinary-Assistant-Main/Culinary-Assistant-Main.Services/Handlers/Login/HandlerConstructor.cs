using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Handlers.Login
{
	public static class HandlerConstructor
	{
		public static BaseLoginHandler GetLoginHandler()
		{
			var phoneLoginHandler = new PhoneLoginHandler();
			var emailLoginHandler = new EmailLoginHandler(phoneLoginHandler);
			var userLoginHandler = new UserLoginHandler(emailLoginHandler);
			return userLoginHandler;
		}
	}
}
