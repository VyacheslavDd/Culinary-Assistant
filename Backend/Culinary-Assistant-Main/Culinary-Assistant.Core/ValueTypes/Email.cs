using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.ValueTypes
{
	public class Email : ValueObject
	{
		public string? Value { get; private set; }

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Value;
		}

		public static Result<Email> Create(string email)
		{
			var isCorrectEmail = Regexes.EmailRegex.Match(email).Success;
			if (isCorrectEmail) return Result.Success(new Email() { Value = email });
			return Result.Failure<Email>("Некорректный формат E-mail.");
		}
	}
}
