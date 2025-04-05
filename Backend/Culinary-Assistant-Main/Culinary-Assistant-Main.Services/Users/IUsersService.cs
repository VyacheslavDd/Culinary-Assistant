using Core.Base.Interfaces;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Users
{
	public interface IUsersService : IService<User, UserInDTO, UpdateUserDTO>
	{
	}
}
