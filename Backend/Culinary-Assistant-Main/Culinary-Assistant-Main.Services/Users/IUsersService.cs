using Core.Base.Interfaces;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.PasswordRecover;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Domain.Models;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Users
{
	public interface IUsersService : IService<User, UserInDTO, UpdateUserDTO>
	{
		Task<Result> UpdatePasswordAsync(Guid userId, UpdatePasswordDTO updatePasswordDTO);
		Task SetPresignedUrlPictureAsync<T>(IMinioClient minioClient, List<T> userOutDTO) where T: IUserOutDTO;
		Task<Result> RecoverPasswordAsync(RecoverPasswordInDTO recoverPasswordInDTO);
	}
}
