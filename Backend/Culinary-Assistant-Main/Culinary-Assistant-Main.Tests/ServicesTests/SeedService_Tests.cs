using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Culinary_Assistant_Main.Services.Seed;
using Culinary_Assistant_Main.Services.Users;
using Culinary_Assistant_Main.Tests.Common;
using Minio;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.ServicesTests
{
	[TestFixture]
	public class SeedService_Tests
	{
		private CulinaryAppContext _culinaryAppContext;
		private ISeedService _seedService;
		private IUsersService _usersService;

		[SetUp]
		public void SetUp()
		{
			_culinaryAppContext = DbContextMocker.CreateInMemoryAppContext();
			var usersRepository = new UsersRepository(_culinaryAppContext);
			var logger = CommonUtils.MockLogger();
			_seedService = new SeedService(usersRepository, logger);
			_usersService = new UsersService(usersRepository, logger);
		}

		[TearDown]
		public void TearDown()
		{
			_culinaryAppContext.Dispose();
		}

		[Test]
		public async Task FirstTimeAdminSeed_CreatesAdmin()
		{
			await _seedService.CreateAdministratorUserAsync();
			var users = await _usersService.GetAllAsync();
			Assert.That(users, Has.Count.EqualTo(1));
			var user = users[0];
			Assert.Multiple(() =>
			{
				Assert.That(user.IsAdmin, Is.True);
				Assert.That(user.Login.Value, Is.EqualTo("Culinary_Perfecto"));
			});
		}

		[Test]
		public async Task RepeatableAdminSeeds_DoNot_CreateAdmin()
		{
			await _seedService.CreateAdministratorUserAsync();
			await _seedService.CreateAdministratorUserAsync();
			await _seedService.CreateAdministratorUserAsync();
			var users = await _usersService.GetAllAsync();
			Assert.That(users, Has.Count.EqualTo(1));
		}
	}
}
