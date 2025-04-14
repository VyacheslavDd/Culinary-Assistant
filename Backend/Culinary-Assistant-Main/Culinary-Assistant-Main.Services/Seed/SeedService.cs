using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.Users;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Seed
{
	public class SeedService(IUsersRepository usersRepository, ILogger logger) : ISeedService
	{
		private readonly IUsersRepository _usersRepository = usersRepository;
		private readonly ILogger _logger = logger;

		public async Task<Guid> CreateAdministratorUserAsync()
		{
			var adminLogin = "Culinary_Perfecto";
			var existingAdmin = await _usersRepository.GetBySelectorAsync(u => u.Login.Value == adminLogin);
			if (existingAdmin != null)
			{
				_logger.Information("Администратор уже создан. Дополнительных действий не требуется");
				return existingAdmin.Id;
			}
			var userDTO = new UserInDTO(adminLogin, "admin@admin.ru", "Culinar_scr");
			var adminUser = User.Create(userDTO);
			adminUser.Value.SetAsAdmin();
			var adminGuid = await _usersRepository.AddAsync(adminUser.Value);
			_logger.Information("Администратор создан: {guid}", adminGuid);
			return adminGuid;
		}
	}
}
