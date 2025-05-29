using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.PasswordRecover;
using Culinary_Assistant.Core.Tests;
using Culinary_Assistant_Notifications_Infrastructure;
using Culinary_Assistant_Notifications_Services.PasswordRecoverService;
using Culinary_Assistant_Notifications_Services.PasswordRecoversService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Notifications.Tests
{
	[TestFixture]
	public class PasswordRecoversService_Tests
	{
		private NotificationsContext _notificationsContext;
		private IPasswordRecoversService _passwordRecoversService;

		[SetUp]
		public void Setup()
		{
			_notificationsContext = DbContextMocker.CreateInMemoryAppContext<NotificationsContext>();
			_passwordRecoversService = new PasswordRecoversService(_notificationsContext);
		}

		[TearDown]
		public void Teardown()
		{
			_notificationsContext.Dispose();
		}

		[Test]
		public async Task CanAddRequest()
		{
			var res = await AddPasswordRecoverRequestAsync();
			var requestsCount = await _passwordRecoversService.GetOverallCountAsync();
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(requestsCount, Is.EqualTo(1));
			});
		}

		[Test]
		public async Task CanGetByRecoverGuid()
		{
			var res = await AddPasswordRecoverRequestAsync();
			var recover = await _passwordRecoversService.GetRowByRecoverIdAsync(res.Value);
			Assert.That(recover, Is.Not.Null);
		}

		[Test]
		public async Task CanUseNotExpiredRequest()
		{
			var res = await AddPasswordRecoverRequestAsync();
			var checkRes = await _passwordRecoversService.CheckLatestRecoverRowForUserByRecoverIdAsync(res.Value);
			Assert.That(checkRes.IsSuccess, Is.True);
		}

		[Test]
		public async Task CanAddRequestWhenTimeBetweenPassed()
		{
			var resFirst = await AddPasswordRecoverRequestAsync();
			await SetExpirationTimeForPasswordRecoverRequestAsync(resFirst.Value, DateTime.UtcNow.AddMinutes(-(MiscellaneousConstants.TimeMinutesBetweenPasswordRecoverMessagesSending)));
			var resSecond = await AddPasswordRecoverRequestAsync();
			Assert.That(resSecond.IsSuccess, Is.True);
		}

		[Test]
		public async Task OrderByExpiresAtMattersWhenCheckingActuality()
		{
			var resFirst = await AddPasswordRecoverRequestAsync();
			await SetExpirationTimeForPasswordRecoverRequestAsync(resFirst.Value, DateTime.UtcNow.AddMinutes(-(MiscellaneousConstants.TimeMinutesBetweenPasswordRecoverMessagesSending)));
			var resSecond = await AddPasswordRecoverRequestAsync();
			var checkOldest = await _passwordRecoversService.CheckLatestRecoverRowForUserByRecoverIdAsync(resFirst.Value);
			var checkLatest = await _passwordRecoversService.CheckLatestRecoverRowForUserByRecoverIdAsync(resSecond.Value);
			Assert.Multiple(() =>
			{
				Assert.That(checkOldest.IsFailure, Is.True);
				Assert.That(checkLatest.IsSuccess, Is.True);
			});
		}

		[Test]
		public async Task CannotAddRequestWhenWrongEmail()
		{
			var res = await AddPasswordRecoverRequestAsync("helo(((@mail,ru");
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public async Task GetByGuidReturnsNullWhenNotExistingRequest()
		{
			var recover = await _passwordRecoversService.GetRowByRecoverIdAsync(Guid.NewGuid());
			Assert.That(recover, Is.Null);
		}

		[Test]
		public async Task CannotUseExpiredRequest()
		{
			var res = await AddPasswordRecoverRequestAsync();
			await SetExpirationTimeForPasswordRecoverRequestAsync(res.Value, DateTime.UtcNow.AddMinutes(-(MiscellaneousConstants.PasswordRecoveryActiveTimeMinutes + 1)));
			var checkRes = await _passwordRecoversService.CheckLatestRecoverRowForUserByRecoverIdAsync(res.Value);
			Assert.That(checkRes.IsFailure, Is.True);
		}

		[Test]
		public async Task CannotCreateTwoRequestsInRow()
		{
			var resFirst = await AddPasswordRecoverRequestAsync();
			var resSecond = await AddPasswordRecoverRequestAsync();
			Assert.Multiple(() =>
			{
				Assert.That(resFirst.IsSuccess, Is.True);
				Assert.That(resSecond.IsFailure, Is.True);
			});
		}

		private async Task<Result<Guid>> AddPasswordRecoverRequestAsync(string email= "hellothere@mail.ru")
		{
			var recoverDTO = new PasswordRecoverInDTO(email);
			var res = await _passwordRecoversService.AddAsync(recoverDTO);
			return res;
		}

		private async Task SetExpirationTimeForPasswordRecoverRequestAsync(Guid recoverId, DateTime expirationTime)
		{
			var recover = await _passwordRecoversService.GetRowByRecoverIdAsync(recoverId);
			if (recover == null) return;
			recover.SetExpiresAt(expirationTime);
			await _notificationsContext.SaveChangesAsync();
		}
	}
}
