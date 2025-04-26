using AutoMapper;
using Culinary_Assistant.Core.DTO.Auth;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Culinary_Assistant_Main.Services.Seed;
using Culinary_Assistant_Main.Services.Users;
using Culinary_Assistant_Main.Tests.Common;
using Microsoft.AspNetCore.Http;
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
		private HttpResponse _httpResponse;
		private IAuthService _authService;
		private IUsersService _usersService;

		[SetUp]
		public async Task SetUp()
		{
			_dbContext = DbContextMocker.CreateInMemoryAppContext();
			_httpResponse = CommonUtils.MockHttpResponse();
			var logger = CommonUtils.MockLogger();
			var usersRepository = new UsersRepository(_dbContext);
			var seedService = new SeedService(usersRepository, logger);
			var configuration = CommonUtils.MockConfiguration();
			await seedService.CreateAdministratorUserAsync();
			var mapper = CommonUtils.MockMapper();
			_authService = new AuthService(usersRepository, configuration, mapper);
			_usersService = new UsersService(usersRepository, logger);
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
			var res = await _authService.RegisterAsync(registerData, _httpResponse);
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
			var res = await _authService.AuthenthicateAsync(authData, _httpResponse);
			Assert.That(res.IsSuccess, Is.True);
		}

		[Test]
		public async Task Can_Authenthicate_WithEmail()
		{
			var authData = new AuthInDTO("admin@admin.ru", "Culinar_scr");
			var res = await _authService.AuthenthicateAsync(authData, _httpResponse);
			Assert.That(res.IsSuccess, Is.True);
		}

		[Test]
		public async Task Can_Authenthicate_WithPhone()
		{
			var adminUser = (await _usersService.GetAllAsync())[0];
			await _usersService.NotBulkUpdateAsync(adminUser.Id, new UpdateUserDTO(null, null, "+75351346688", null));
			var authData = new AuthInDTO("85351346688", "Culinar_scr");
			var res = await _authService.AuthenthicateAsync(authData, _httpResponse);
			Assert.That(res.IsSuccess, Is.True);
		}

		[Test]
		public async Task CannotAuthenthicate_With_WrongLogin()
		{
			var authData = new AuthInDTO("Culinary_Perfect", "Culinar_scr");
			var res = await _authService.AuthenthicateAsync(authData, _httpResponse);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task StandardUser_CannotAuthenthicate_AsAdmin()
		{
			await _authService.RegisterAsync(new UserInDTO("someuser", "user@user.ru", "somepassword"), _httpResponse);
			var authData = new AuthInDTO("someuser", "somepassword", AdminEntrance: true);
			var res = await _authService.AuthenthicateAsync(authData, _httpResponse);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotAuthenthicate_With_WrongPassword()
		{
			var authData = new AuthInDTO("Culinary_Perfecto", "Culinar_scr1");
			var res = await _authService.AuthenthicateAsync(authData, _httpResponse);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotRegister_With_ConflictingData()
		{
			var registerData = new UserInDTO("Culinary_Perfecto", "admin@admin.ru", "Culinar_scr");
			var res = await _authService.RegisterAsync(registerData, _httpResponse);
			Assert.That(res.IsFailure, Is.True);
		}
	}
}
