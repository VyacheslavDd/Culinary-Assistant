using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.PasswordRecover;
using Culinary_Assistant_Notifications_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Notifications_Services.PasswordRecoverService
{
	public interface IPasswordRecoversService
	{
		Task<int> DeleteOutdatedRecoversAsync();
		Task<Result<Guid>> AddAsync(PasswordRecoverInDTO passwordRecoverInDTO);
		Task<PasswordRecover?> GetRowByRecoverIdAsync(Guid id);
		Task<Result<PasswordRecover>> CheckLatestRecoverRowForUserByRecoverIdAsync(Guid id);
		Task<int> GetOverallCountAsync();
	}
}
