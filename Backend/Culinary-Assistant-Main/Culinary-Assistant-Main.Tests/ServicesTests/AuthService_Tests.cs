using Culinary_Assistant.Core.DTO.Auth;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Culinary_Assistant_Main.Services.Seed;
using Culinary_Assistant_Main.Services.Users;
using Culinary_Assistant_Main.Tests.Common;
using Minio;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.ServicesTests
{
	[TestFixture]
	public class AuthService_Tests
	{
		private CulinaryAppContext _dbContext;

		private IAuthService _authService;
		private IUsersService _usersService;

		[SetUp]
		public async Task SetUp()
		{
			_dbContext = DbContextMocker.CreateInMemoryAppContext();
			var logger = CommonUtils.MockLogger();
			var usersRepository = new UsersRepository(_dbContext);
			var seedService = new SeedService(usersRepository, logger);
			await seedService.CreateAdministratorUserAsync();
			_authService = new AuthService(usersRepository);
			var minioClientFactory = new Mock<IMinioClientFactory>();
			_usersService = new UsersService(usersRepository, logger, minioClientFactory.Object);
		}

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}

		[Test]
		public async Task Can_Register()
		{
			var registerData = new UserInDTO("aya334", "hello@mail.ru", "aya334exe");
			var res = await _authService.RegisterAsync(registerData);
			var users = await _usersService.GetAllAsync();
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(users, Has.Count.EqualTo(2));
			});
		}

		[Test]
		public async Task Can_Authenthicate_WithLogin()
		{
			var authData = new AuthInDTO("Culinary_Perfecto", "Culinar_scr");
			var res = await _authService.AuthenthicateAsync(authData);
			Assert.That(res.IsSuccess, Is.True);
		}

		[Test]
		public async Task Can_Authenthicate_WithEmail()
		{
			var authData = new AuthInDTO("admin@admin.ru", "Culinar_scr");
			var res = await _authService.AuthenthicateAsync(authData);
			Assert.That(res.IsSuccess, Is.True);
		}

		[Test]
		public async Task Can_Authenthicate_WithPhone()
		{
			var adminUser = (await _usersService.GetAllAsync())[0];
			await _usersService.NotBulkUpdateAsync(adminUser.Id, new UpdateUserDTO(null, null, "+75351346688", null));
			var authData = new AuthInDTO("85351346688", "Culinar_scr");
			var res = await _authService.AuthenthicateAsync(authData);
			Assert.That(res.IsSuccess, Is.True);
		}

		[Test]
		public async Task CannotAuthenthicate_With_WrongLogin()
		{
			var authData = new AuthInDTO("Culinary_Perfect", "Culinar_scr");
			var res = await _authService.AuthenthicateAsync(authData);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task StandardUser_CannotAuthenthicate_AsAdmin()
		{
			await _authService.RegisterAsync(new UserInDTO("someuser", "user@user.ru", "somepassword"));
			var authData = new AuthInDTO("someuser", "somepassword", AdminEntrance: true);
			var res = await _authService.AuthenthicateAsync(authData);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotAuthenthicate_With_WrongPassword()
		{
			var authData = new AuthInDTO("Culinary_Perfecto", "Culinar_scr1");
			var res = await _authService.AuthenthicateAsync(authData);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotRegister_With_ConflictingData()
		{
			var registerData = new UserInDTO("Culinary_Perfecto", "admin@admin.ru", "Culinar_scr");
			var res = await _authService.RegisterAsync(registerData);
			Assert.That(res.IsFailure, Is.True);
		}
	}
}
