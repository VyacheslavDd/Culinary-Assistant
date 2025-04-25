using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Handlers.Login
{
	public class PhoneLoginHandler(BaseLoginHandler? nextLoginHandler = null) : BaseLoginHandler(nextLoginHandler)
	{
		public override async Task<User?> Handle(IUsersRepository usersRepository, string loginValue)
		{
			var isPhone = long.TryParse(loginValue.Replace("+7", "8"), out long phone);
			if (!isPhone) return await HandleNext(usersRepository, loginValue);
			var user = await usersRepository.GetBySelectorAsync(u => u.Phone.Value == phone);
			if (user == null) return await HandleNext(usersRepository, loginValue);
			return user;
		}
	}
}
