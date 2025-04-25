using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.Data
{
	public static class UsersData
	{
		public static List<User> Users =>
			[
				User.Create(new UserInDTO("mybestlogin", "emailhere@email.ru", "anton553f")).Value,
				User.Create(new UserInDTO("myworstlogin", "worst@best.com", "DONTREMEMBER")).Value,
				User.Create(new UserInDTO("home_money", "88005553535", "reference_xd")).Value
			];
	}
}
