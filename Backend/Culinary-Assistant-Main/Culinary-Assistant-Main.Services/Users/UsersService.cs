using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Users
{
	public class UsersService(IUsersRepository usersRepository, ILogger logger) :
		BaseService<User, UserInDTO, UpdateUserDTO>(usersRepository, logger), IUsersService
	{
		public override Task<Result<Guid>> CreateAsync(UserInDTO entityCreateRequest, bool autoSave = true)
		{
			throw new NotImplementedException();
		}
	}
}
