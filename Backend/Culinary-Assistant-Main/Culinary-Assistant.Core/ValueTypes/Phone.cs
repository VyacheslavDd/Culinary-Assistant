using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.ValueTypes
{
	public class Phone : ValueObject
	{
		public long? Value { get; private set; }

		public static Result<Phone> Create(string phone)
		{
			var isCorrectPhone = Regexes.PhoneRegex.Match(phone).Success;
			if (isCorrectPhone)
			{
				var phoneToLong = long.Parse(phone.Replace("+7", "8"));
				return Result.Success(new Phone() { Value = phoneToLong });
			}
			return Result.Failure<Phone>("Номер должен начинаться с +7 или 8 и быть длиною 11 цифр");
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Value;
		}
	}
}
