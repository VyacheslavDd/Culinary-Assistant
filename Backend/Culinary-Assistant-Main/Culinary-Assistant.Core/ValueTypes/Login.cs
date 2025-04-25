using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.ValueTypes
{
	public class Login : ValueObject
	{
		public string Value { get; private set; }

		public static Result<Login> Create(string login)
		{
			var isCorrectLogin = Regexes.LoginRegex.Match(login).Success;
			if (isCorrectLogin) return Result.Success(new Login() { Value = login });
			return Result.Failure<Login>("Логин должен быть длиной 1-25 символов\nНачинаться с буквы и заканчиваться буквой или цифрой\nСодержать буквы и цифры, символ _");
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Value;
		}
	}
}
