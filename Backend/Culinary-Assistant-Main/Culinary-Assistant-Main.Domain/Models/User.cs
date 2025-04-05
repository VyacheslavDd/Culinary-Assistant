using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models
{
	public class User : Core.Base.Entity<Guid>
	{
		private readonly List<Receipt> _receipts = [];

		public Login Login { get; private set; }
		public Phone Phone { get; private set; }
		public Email Email { get; private set; }
		public string? ProfilePictureUrl { get; private set; }
		public string PasswordHash { get; private set; }
		public IReadOnlyCollection<Receipt> Receipts => _receipts;

		public static Result<User> Create(UserInDTO userInDTO)
		{
			var loginObject = Login.Create(userInDTO.Login);
			if (loginObject.IsFailure) return Result.Failure<User>(loginObject.Error);
			var phone = Phone.Create(userInDTO.EmailOrPhone);
			var email = Email.Create(userInDTO.EmailOrPhone);
			if (phone.IsFailure && email.IsFailure) return Result.Failure<User>($"{phone.Error} \n {email.Error}");
			var user = new User
			{
				Login = loginObject.Value
			};
			if (phone.IsSuccess)
			{
				user.Phone = phone.Value;
			}
			else
			{
				user.Email = email.Value;
			}
			user.SetPassword(userInDTO.Password);
			return Result.Success(user);
		}

		public void SetProfilePictureUrl(string newUrl)
		{
			ProfilePictureUrl = newUrl;
		}

		public void SetPassword(string password)
		{
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
		}

		public Result SetLogin(string login)
		{
			var result = Login.Create(login);
			if (result.IsFailure) return Result.Failure(result.Error);
			Login = result.Value;
			return Result.Success();
		}

		public Result SetPhone(string phone)
		{
			var result = Phone.Create(phone);
			if (result.IsFailure) return Result.Failure(result.Error);
			Phone = result.Value;
			return Result.Success();
		}

		public Result SetEmail(string email)
		{
			var result = Email.Create(email);
			if (result.IsFailure) return Result.Failure(result.Error);
			Email = result.Value;
			return Result.Success();
		}

		public void AddReceipt(Receipt receipt)
		{
			_receipts.Add(receipt);
		}
	}
}
