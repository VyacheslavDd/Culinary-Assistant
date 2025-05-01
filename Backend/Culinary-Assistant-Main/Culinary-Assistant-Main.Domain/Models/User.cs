using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Utils;
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
		private readonly List<ReceiptCollection> _receiptCollections = [];
		private readonly List<ReceiptLike> _receiptLikes = [];
		private readonly List<ReceiptCollectionLike> _receiptCollectionLikes = [];
		private readonly List<ReceiptFavourite> _favouritedReceipts = [];

		public Login Login { get; private set; }
		public Phone Phone { get; private set; }
		public Email Email { get; private set; }
		public string? ProfilePictureUrl { get; private set; }
		public bool IsAdmin { get; private set; }
		public string PasswordHash { get; private set; }
		public IReadOnlyCollection<Receipt> Receipts => _receipts;
		public IReadOnlyCollection<ReceiptCollection> ReceiptCollections => _receiptCollections;
		public IReadOnlyCollection<ReceiptLike> ReceiptLikes => _receiptLikes;
		public IReadOnlyCollection<ReceiptCollectionLike> ReceiptCollectionLikes => _receiptCollectionLikes;
		public IReadOnlyCollection<ReceiptFavourite> FavouritedReceipts => _favouritedReceipts;

		public static Result<User> Create(UserInDTO userInDTO)
		{
			var loginObject = Login.Create(userInDTO.Login);
			if (loginObject.IsFailure) return Result.Failure<User>(loginObject.Error);
			var phone = Phone.Create(userInDTO.EmailOrPhone);
			var email = Email.Create(userInDTO.EmailOrPhone);
			if (phone.IsFailure && email.IsFailure) return Result.Failure<User>($"Если телефон, то {phone.Error}\nЕсли почта, то {email.Error}");
			var user = new User
			{
				Login = loginObject.Value
			};
			if (phone.IsSuccess)
			{
				user.Email = new Email();
				user.Phone = phone.Value;
			}
			else
			{
				user.Phone = new Phone();
				user.Email = email.Value;
			}
			var passRes = user.SetPassword(userInDTO.Password);
			if (passRes.IsFailure) return Result.Failure<User>(passRes.Error);
			return Result.Success(user);
		}

		public void SetProfilePictureUrl(string? newUrl)
		{
			ProfilePictureUrl = newUrl;
		}

		public Result SetPassword(string password)
		{
			var matches = Regexes.PasswordRegex.Match(password);
			if (!matches.Success) return Result.Failure("Пароль не соответствует указанным требованиям");
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
			return Result.Success();
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

		public void SetAsAdmin()
		{
			IsAdmin = true;
		}

		public void AddReceipt(Receipt receipt)
		{
			_receipts.Add(receipt);
		}
	}
}
