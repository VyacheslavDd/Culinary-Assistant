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
	public abstract class BaseLoginHandler(BaseLoginHandler? next = null)
	{
		private readonly BaseLoginHandler? _next = next;

		protected async Task<User?> HandleNext(IUsersRepository usersRepository, string loginValue)
		{
			if (_next == null) return null;
			return await _next.Handle(usersRepository, loginValue);
		}

		public abstract Task<User?> Handle(IUsersRepository usersRepository, string loginValue);
	}
}
