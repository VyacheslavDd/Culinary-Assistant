using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.PasswordRecover;
using Culinary_Assistant_Notifications_Domain.Models;
using Culinary_Assistant_Notifications_Infrastructure;
using Culinary_Assistant_Notifications_Services.PasswordRecoverService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Notifications_Services.PasswordRecoversService
{
	public class PasswordRecoversService(NotificationsContext notificationsContext) : IPasswordRecoversService
	{
		private readonly NotificationsContext _notificationsContext = notificationsContext;

		public async Task<Result<Guid>> AddAsync(PasswordRecoverInDTO passwordRecoverInDTO)
		{
			var passwordRecoverResult = PasswordRecover.Create(passwordRecoverInDTO);
			if (passwordRecoverResult.IsFailure) return Result.Failure<Guid>(passwordRecoverResult.Error);
			var canSendNewOne = await CheckCanSendNewRecoveryMessageAsync(passwordRecoverResult.Value);
			if (!canSendNewOne) return Result.Failure<Guid>("Прошло слишком мало времени между отправками запросов на смену пароля");
			var entityInfo = await _notificationsContext.PasswordRecovers.AddAsync(passwordRecoverResult.Value);
			await _notificationsContext.SaveChangesAsync();
			return Result.Success((Guid)entityInfo.Property("RecoverId").CurrentValue);
		}

		public async Task<int> GetOverallCountAsync()
		{
			return await _notificationsContext.PasswordRecovers.CountAsync();
		}

		public async Task<int> DeleteOutdatedRecoversAsync()
		{
			return await _notificationsContext.PasswordRecovers
				.Where(pr => (DateTime.UtcNow - pr.ExpiresAt).Minutes >= MiscellaneousConstants.TimeMinutesDifferenceRequiredToOutdatePasswordRecoveryMessage)
				.ExecuteDeleteAsync();
		}

		public async Task<Result<PasswordRecover>> CheckLatestRecoverRowForUserByRecoverIdAsync(Guid id)
		{
			var recoveryRow = await GetRowByRecoverIdAsync(id);
			if (recoveryRow == null) return Result.Failure<PasswordRecover>("Истек срок смены пароля или запроса не существует");
			var latestRecoveryForUser = await GetLatestRecoveryForUser(recoveryRow);
			if (latestRecoveryForUser == null || latestRecoveryForUser.RecoverId != id || DateTime.UtcNow > latestRecoveryForUser.ExpiresAt)
				return Result.Failure<PasswordRecover>("Запрос не актуален, перезапросите смену пароля");
			return Result.Success(latestRecoveryForUser);
		}

		public async Task<PasswordRecover?> GetRowByRecoverIdAsync(Guid id)
		{
			return await _notificationsContext.PasswordRecovers.FirstOrDefaultAsync(pr => pr.RecoverId == id);
		}

		private async Task<bool> CheckCanSendNewRecoveryMessageAsync(PasswordRecover passwordRecover)
		{
			var latestRecoveryForUser = await GetLatestRecoveryForUser(passwordRecover);
			return latestRecoveryForUser == null ||
			 (DateTime.UtcNow.AddMinutes(MiscellaneousConstants.PasswordRecoveryActiveTimeMinutes) - latestRecoveryForUser.ExpiresAt).Minutes >= MiscellaneousConstants.TimeMinutesBetweenPasswordRecoverMessagesSending;
		}

		private async Task<PasswordRecover?> GetLatestRecoveryForUser(PasswordRecover recoveryRow)
		{
			return await _notificationsContext.PasswordRecovers
				.Where(pr => pr.UserEmail.Value == recoveryRow.UserEmail.Value)
				.OrderByDescending(pr => pr.ExpiresAt)
				.FirstOrDefaultAsync();
		}
	}
}
