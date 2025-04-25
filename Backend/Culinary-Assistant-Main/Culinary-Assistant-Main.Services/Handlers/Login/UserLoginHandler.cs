using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Handlers.Login
{
	public class UserLoginHandler(BaseLoginHandler? nextLoginHandler = null) : BaseLoginHandler(nextLoginHandler)
	{
		public override async Task<User?> Handle(IUsersRepository usersRepository, string loginValue)
		{
			var user = await usersRepository.GetBySelectorAsync(u => u.Login.Value == loginValue);
			if (user == null) return await HandleNext(usersRepository, loginValue);
			return user;
		}
	}
}
