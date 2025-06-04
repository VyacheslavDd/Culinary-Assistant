using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.DTO.PasswordRecover;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Http.Service;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant.Core.ValueTypes;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Minio;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Users
{
	public class UsersService(IUsersRepository usersRepository, ILogger logger, IHttpClientService httpClientService, IConfiguration configuration) :
		BaseService<User, UserInDTO, UpdateUserDTO>(usersRepository, logger), IUsersService
	{
		private readonly IHttpClientService _httpClientService = httpClientService;
		private readonly string _notificationsHttpClientName = configuration[ConfigurationConstants.NotificationsHttpClientName]!;

		public override async Task<User?> GetByGuidAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var user = await base.GetByGuidAsync(id, cancellationToken);
			if (user != null)
			{
				await _repository.LoadCollectionAsync(user, u => u.Receipts);
				await _repository.LoadCollectionAsync(user, u => u.ReceiptCollections);
			}
			return user;
		}

		public override async Task<Result> NotBulkUpdateAsync(Guid entityId, UpdateUserDTO updateRequest)
		{
			var user = await _repository.GetBySelectorAsync(u => u.Id == entityId);
			if (user == null)
				return Result.Failure("Попытка обновить несуществующего пользователя");
			var results = Miscellaneous.CreateResultList(3);
			results[0] = user.SetLogin(updateRequest.Login ?? user.Login.Value);
			if (updateRequest.Email != null)
				results[1] = user.SetEmail(updateRequest.Email);
			if (updateRequest.Phone != null)
				results[2] = user.SetPhone(updateRequest.Phone);
			user.SetProfilePictureUrl(updateRequest?.ProfilePictureUrl ?? user.ProfilePictureUrl);
			if (!results.All(r => r.IsSuccess))
				return Miscellaneous.ResultFailureWithAllFailuresFromResultList(results);
			return await base.NotBulkUpdateAsync(entityId, updateRequest);
		}

		public async Task<Result> UpdatePasswordAsync(Guid userId, UpdatePasswordDTO updatePasswordDTO)
		{
			var user = await _repository.GetBySelectorAsync(u => u.Id == userId);
			if (user == null)
				return Result.Failure("Попытка обновить несуществующего пользователя");
			var isPasswordVerified = BCrypt.Net.BCrypt.Verify(updatePasswordDTO.OldPassword, user.PasswordHash);
			if (!isPasswordVerified)
				return Result.Failure("Некорректно введен старый пароль");
			return await SetNewPasswordAsync(user, updatePasswordDTO);
		}

		public async Task SetPresignedUrlPictureAsync<T>(IMinioClient minioClient, List<T> userOutDTO) where T: IUserOutDTO
		{
			var filePaths = userOutDTO.Select(uDTO => new FilePath(uDTO.PictureUrl ?? "")).ToList();
			var presignedUrls = await MinioUtils.GetPresignedUrlsForFilesFromFilePathsAsync(minioClient, _logger, filePaths);
			for (var i = 0; i < presignedUrls.Count; i++)
				userOutDTO[i].PictureUrl = presignedUrls[i].Url;
		}

		public override Task<Result<Guid>> CreateAsync(UserInDTO entityCreateRequest, bool autoSave = true)
		{
			throw new NotSupportedException();
		}

		public async Task<Result> RecoverPasswordAsync(RecoverPasswordInDTO recoverPasswordInDTO)
		{
			var response = await _httpClientService.GetAsync(_notificationsHttpClientName, $"api/password-recovers/{recoverPasswordInDTO.RecoverId}");
			if (response == null || !response.IsSuccessStatusCode) return Result.Failure("Срок смены пароля истек или используется неактуальный запрос. Запросите новую попытку.");
			var emailValue = await response.Content.ReadAsStringAsync();
			var user = await _repository.GetBySelectorAsync(u => u.Email.Value == emailValue);
			if (user == null) return Result.Failure("Смена пароля осуществляется для несуществующего пользователя");
			return await SetNewPasswordAsync(user, recoverPasswordInDTO);
		}

		private async Task<Result> SetNewPasswordAsync(User user, IUpdatePasswordDTO updatePasswordDTO)
		{
			if (updatePasswordDTO.NewPassword != updatePasswordDTO.NewPasswordConfirmation)
				return Result.Failure("Подтверждение нового пароля некорректно");
			var res = user.SetPassword(updatePasswordDTO.NewPassword);
			if (res.IsFailure)
				return Result.Failure(res.Error);
			await SaveChangesAsync();
			return Result.Success();
		}
	}
}
