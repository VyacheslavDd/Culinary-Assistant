using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.ModelsTests
{
	[TestFixture]
	public class UserModel_Tests
	{
		private User _user;

		[SetUp]
		public void CreateTestUser()
		{
			_user = User.Create(new UserInDTO("test", "test@test.ru", "hellotherefriend")).Value;
		}

		[Test]
		public void CreateUser_WorksCorrectly_WithCorrectLoginAndEmail()
		{
			var result = User.Create(new UserInDTO("login334", "hey@mail.ru", "hellotherefriend"));
			Assert.Multiple(() =>
			{
				Assert.That(result.IsSuccess, Is.True);
				Assert.That(result.Value.Login.Value, Is.EqualTo("login334"));
				Assert.That(result.Value.Email.Value, Is.EqualTo("hey@mail.ru"));
			});
		}

		[Test]
		public void CreateUser_WorksCorrectly_WithCorrectLoginAndPhone()
		{
			var result = User.Create(new UserInDTO("login334", "89930159670", "hellotherefriend"));
			Assert.Multiple(() =>
			{
				Assert.That(result.IsSuccess, Is.True);
				Assert.That(result.Value.Login.Value, Is.EqualTo("login334"));
				Assert.That(result.Value.Phone.Value, Is.EqualTo(89930159670));
			});
		}

		[Test]
		public void CreateUser_ReturnsFailure_WithWrongLogin()
		{
			var result = User.Create(new UserInDTO("h", "hey@mail.ru", "hellotherefriend"));
			Assert.That(result.IsFailure, Is.True);
		}

		[Test]
		public void CreateUser_ReturnsFailure_WithBothWrongEmailAndPhone()
		{
			var result = User.Create(new UserInDTO("login", "wtf", "hellotherefriend"));
			Assert.That(result.IsFailure, Is.True);
		}

		[Test]
		public void SetLoginWorksCorrectly_WithCorrectLogin()
		{
			var result = _user.SetLogin("newlogin");
			Assert.Multiple(() =>
			{
				Assert.That(result.IsSuccess, Is.True);
				Assert.That(_user.Login.Value, Is.EqualTo("newlogin"));
			});
		}

		[Test]
		public void SetLoginReturnsFailure_WithWrongLogin()
		{
			var result = _user.SetLogin("newlogin----");
			Assert.That(result.IsFailure, Is.True);
		}

		[Test]
		public void SetPhoneWorksCorrectly_WithCorrectPhone()
		{
			var result = _user.SetPhone("+70123456789");
			Assert.Multiple(() =>
			{
				Assert.That(result.IsSuccess, Is.True);
				Assert.That(_user.Phone.Value, Is.EqualTo(80123456789));
			});
		}

		[Test]
		public void SetPhoneReturnsFailure_WithWrongPhone()
		{
			var result = _user.SetPhone("PHONE");
			Assert.That(result.IsFailure, Is.True);
		}

		[Test]
		public void SetEmailWorksCorrectly_WithCorrectEmail()
		{
			var result = _user.SetEmail("hey@gmail.com");
			Assert.Multiple(() =>
			{
				Assert.That(result.IsSuccess, Is.True);
				Assert.That(_user.Email.Value, Is.EqualTo("hey@gmail.com"));
			});
		}

		[Test]
		public void SetEmailReturnsFailure_WithWrongEmail()
		{
			var result = _user.SetEmail("noemail");
			Assert.That(result.IsFailure, Is.True);
		}

		[Test]
		public void SetProfilePictureUrl_Works()
		{
			_user.SetProfilePictureUrl("UPDATE");
			Assert.That(_user.ProfilePictureUrl, Is.EqualTo("UPDATE"));
		}

		[Test]
		public void SetPassword_Works()
		{
			var hashBefore = _user.PasswordHash;
			_user.SetPassword("password334");
			Assert.That(_user.PasswordHash, Is.Not.EqualTo(hashBefore));
		}

		[Test]
		public void AddReceipt_Works()
		{
			var collectionBeforeCount = _user.Receipts.Count;
			_user.AddReceipt(new Receipt());
			var nextCollection = _user.Receipts;
			Assert.Multiple(() =>
			{
				Assert.That(nextCollection, Has.Count.EqualTo(1));
				Assert.That(collectionBeforeCount, Is.Not.EqualTo(nextCollection.Count));
			});
		}
	}
}
