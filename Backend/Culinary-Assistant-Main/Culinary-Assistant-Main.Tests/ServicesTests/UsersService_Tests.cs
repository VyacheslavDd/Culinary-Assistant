using Culinary_Assistant.Core.DTO.Auth;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.Users;
using Culinary_Assistant_Main.Tests.Common;
using Culinary_Assistant_Main.Tests.Data;
using Minio;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Culinary_Assistant.Core.Enums;

namespace Culinary_Assistant_Main.Tests.ServicesTests
{
	public class UsersService_Tests
	{
		private CulinaryAppContext _dbContext;
		private IUsersService _usersService;
		private IAuthService _authService;

		[SetUp]
		public async Task SetUp()
		{
			_dbContext = DbContextMocker.CreateInMemoryAppContext();
			var logger = CommonUtils.MockLogger();
			var usersRepository = new UsersRepository(_dbContext);
			var configuration = CommonUtils.MockConfiguration();
			var mapper = CommonUtils.MockMapper();
			_usersService = new UsersService(usersRepository, logger);
			_authService = new AuthService(usersRepository, configuration, mapper);
			await CreateSomeUsersAsync();
		}

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}

		[Test]
		public async Task GetAllUsers_WorksCorrectly()
		{
			var users = await _usersService.GetAllAsync();
			Assert.That(users, Has.Count.EqualTo(3));
		}

		[Test]
		public async Task GetByGuid_WorksCorrectly()
		{
			var userId = await GetUserIdAsync();
			var user = await _usersService.GetByGuidAsync(userId);
			Assert.Multiple(() =>
			{
				Assert.That(user, Is.Not.Null);
				Assert.That(user.Login.Value, Is.EqualTo("myworstlogin"));
			});
		}

		[Test]
		public async Task GetByGuid_Includes_ReceiptsAndCollections()
		{
			var userId = await GetUserIdAsync();
			var receipts = ReceiptsData.Receipts;
			var receiptCollection = ReceiptCollection.Create(new ReceiptCollectionInModelDTO("First", false, Color.Red, userId, null)).Value;
			receipts.ForEach(r => r.SetUserId(userId));
			await _dbContext.Receipts.AddRangeAsync(receipts);
			await _dbContext.ReceiptCollections.AddAsync(receiptCollection);
			await _dbContext.SaveChangesAsync();
			var user = await _usersService.GetByGuidAsync(userId);
			Assert.Multiple(() =>
			{
				Assert.That(user.Receipts, Has.Count.EqualTo(3));
				Assert.That(user.ReceiptCollections, Has.Count.EqualTo(1));
			});
		}



		[Test]
		public async Task GetByGuid_ReturnsNull_OnUnknownUser()
		{
			var user = await _usersService.GetByGuidAsync(Guid.Empty);
			Assert.That(user, Is.Null);
		}

		[Test]
		public async Task UpdatingUser_WorksCorrectly()
		{
			var userId = await GetUserIdAsync();
			var response = await _usersService.NotBulkUpdateAsync(userId, new UpdateUserDTO("loginew", "enew@new.ru", "89920159569", "no"));
			var user = await _usersService.GetByGuidAsync(userId);
			Assert.Multiple(() =>
			{
				Assert.That(response.IsSuccess, Is.True);
				Assert.That(user.Login.Value, Is.EqualTo("loginew"));
				Assert.That(user.Email.Value, Is.EqualTo("enew@new.ru"));
				Assert.That(user.Phone.Value, Is.EqualTo(89920159569));
				Assert.That(user.ProfilePictureUrl, Is.EqualTo("no"));
			});
		}

		[Test]
		public async Task DeletingUser_WorksCorrectly()
		{
			var userId = await GetUserIdAsync();
			var response = await _usersService.NotBulkDeleteAsync(userId);
			var users = await _usersService.GetAllAsync();
			Assert.Multiple(() =>
			{
				Assert.That(response.IsSuccess, Is.True);
				Assert.That(users, Has.Count.EqualTo(2));
			});
		}

		[Test]
		public async Task UpdatingPassword_WorksCorrectly()
		{
			var httpResponse = CommonUtils.MockHttpResponse();
			var passwordDTO = new UpdatePasswordDTO("DONTREMEMBER", "NOWREMEMBER", "NOWREMEMBER");
			var userId = await GetUserIdAsync();
			await _usersService.UpdatePasswordAsync(userId, passwordDTO);
			var authResult = await _authService.AuthenthicateAsync(new AuthInDTO("myworstlogin", "NOWREMEMBER"), httpResponse);
			Assert.That(authResult.IsSuccess, Is.True);
		}

		[Test]
		public async Task PartialUpdating_DoesNot_TouchAllData()
		{
			var userId = await GetUserIdAsync();
			var response = await _usersService.NotBulkUpdateAsync(userId, new UpdateUserDTO("newlogin", null, null, null));
			var user = await _usersService.GetByGuidAsync(userId);
			Assert.Multiple(() =>
			{
				Assert.That(response.IsSuccess, Is.True);
				Assert.That(user.Login.Value, Is.EqualTo("newlogin"));
				Assert.That(user.Email.Value, Is.EqualTo("worst@best.com"));
				Assert.That(user.Phone.Value, Is.Null);
				Assert.That(user.ProfilePictureUrl, Is.Null);
			});
		}

		[Test]
		public async Task Cannot_UpdatePassword_If_OldIsWrong()
		{
			var userId = await GetUserIdAsync();
			var passwordDTO = new UpdatePasswordDTO("NOWREMEMBER", "NEWPASSWORD", "NEWPASSWORD");
			var response = await _usersService.UpdatePasswordAsync(userId, passwordDTO);
			Assert.That(response.IsFailure, Is.True);
		}

		[Test]
		public async Task Cannot_UpdatePassword_If_ConfirmationIsWrong()
		{
			var userId = await GetUserIdAsync();
			var passwordDTO = new UpdatePasswordDTO("DONTREMEMBER", "NEWPASSWORD", "NOWPASSWORD");
			var response = await _usersService.UpdatePasswordAsync(userId, passwordDTO);
			Assert.That(response.IsFailure, Is.True);
		}

		[Test]
		public async Task UpdatingFailures_OnBadRequest()
		{
			var userId = await GetUserIdAsync();
			var response = await _usersService.NotBulkUpdateAsync(userId, new UpdateUserDTO("loginew", "rip", "PHONENUMBER...", "no"));
			Assert.That(response.IsFailure, Is.True);
		}

		[Test]
		public async Task UpdatingFailures_OnUpdatingUnknownUser()
		{
			var response = await _usersService.NotBulkUpdateAsync(Guid.Empty, new UpdateUserDTO("cant", "update@user.ru", "85535555785", "picture.com"));
			Assert.That(response.IsFailure, Is.True);
		}

		private async Task CreateSomeUsersAsync()
		{
			await _dbContext.Users.AddRangeAsync(UsersData.Users);
			await _dbContext.SaveChangesAsync();
		}

		private async Task<Guid> GetUserIdAsync()
		{
			return (await _usersService.GetAllAsync())[1].Id;
		}
	}
}
