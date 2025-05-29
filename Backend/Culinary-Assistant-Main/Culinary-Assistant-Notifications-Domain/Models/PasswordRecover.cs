using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.PasswordRecover;
using Culinary_Assistant.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Notifications_Domain.Models
{
	public class PasswordRecover : Core.Base.Entity<Guid>
	{
		public Email UserEmail { get; private set; }
		public Guid RecoverId { get; private set; }
		public DateTime ExpiresAt { get; private set; }

		public static Result<PasswordRecover> Create(PasswordRecoverInDTO passwordRecoverInDTO)
		{
			var emailResult = Email.Create(passwordRecoverInDTO.Email);
			if (emailResult.IsFailure) return Result.Failure<PasswordRecover>(emailResult.Error);
			var passwordRecover = new PasswordRecover
			{
				UserEmail = emailResult.Value,
				RecoverId = Guid.NewGuid(),
				ExpiresAt = DateTime.UtcNow.AddMinutes(MiscellaneousConstants.PasswordRecoveryActiveTimeMinutes)
			};
			return Result.Success(passwordRecover);
		}

		/// <summary>
		/// Сменить дату окончания действия смены пароля. Использовать только для тестирования!
		/// </summary>
		/// <param name="dateTime">Дата, когда запрос смены пароля перестанет действовать</param>
		public void SetExpiresAt(DateTime dateTime)
		{
			ExpiresAt = dateTime;
		}
	}
}
