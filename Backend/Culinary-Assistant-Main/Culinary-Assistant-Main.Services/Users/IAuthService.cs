using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Auth;
using Culinary_Assistant.Core.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Users
{
	public interface IAuthService
	{
		Task<Result<AuthOutDTO>> RegisterAsync(UserInDTO userInDTO);
		Task<Result<AuthOutDTO>> AuthenthicateAsync(AuthInDTO authInDTO);
	}
}
