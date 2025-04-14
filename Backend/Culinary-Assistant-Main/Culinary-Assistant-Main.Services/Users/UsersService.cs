using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant.Core.ValueTypes;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Minio;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Users
{
	public class UsersService(IUsersRepository usersRepository, ILogger logger, IMinioClientFactory minioClientFactory) :
		BaseService<User, UserInDTO, UpdateUserDTO>(usersRepository, logger), IUsersService
	{
		private readonly IMinioClientFactory _minioClientFactory = minioClientFactory;

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
			if (updatePasswordDTO.NewPassword != updatePasswordDTO.NewPasswordConfirmation)
				return Result.Failure("Подтверждение нового пароля некорректно");
			var res = user.SetPassword(updatePasswordDTO.NewPassword);
			if (res.IsFailure)
				return Result.Failure(res.Error);
			await SaveChangesAsync();
			return Result.Success();
		}

		public async Task SetPresignedUrlPictureAsync<T>(List<T> userOutDTO) where T: IUserOutDTO
		{
			using var minioClient = _minioClientFactory.CreateClient();
			var filePaths = userOutDTO.Select(uDTO => new FilePath(uDTO.PictureUrl ?? "")).ToList();
			var presignedUrls = await MinioUtils.GetPresignedUrlsForFilesFromFilePathsAsync(minioClient, _logger, filePaths);
			for (var i = 0; i < presignedUrls.Count; i++)
				userOutDTO[i].PictureUrl = presignedUrls[i].Url;
		}

		public override Task<Result<Guid>> CreateAsync(UserInDTO entityCreateRequest, bool autoSave = true)
		{
			throw new NotSupportedException();
		}
	}
}
