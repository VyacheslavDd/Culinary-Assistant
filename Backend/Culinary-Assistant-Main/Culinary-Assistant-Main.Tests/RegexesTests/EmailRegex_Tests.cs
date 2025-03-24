using Culinary_Assistant.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.RegexesTests
{
	[TestFixture]
	public class EmailRegex_Tests
	{

		[TestCase("hello@gmail.com")]
		[TestCase("hey@yandex.ru")]
		[TestCase("hey@mail.ru")]
		[TestCase("h343@gmail.com")]
		[TestCase("hjohn.there@mail.com")]
		[TestCase("hjohn_there@mail.com")]
		public void CorrectEmail_ShouldBeMatchedSuccessfully(string email)
		{
			var isSuccess = Regexes.EmailRegex.Match(email).Success;
			Assert.That(isSuccess, Is.True);
		}

		[TestCase("@gmail.com")]
		[TestCase("heytheregmail.com")]
		[TestCase("hey@gmail")]
		[TestCase("hey@gma(il.com")]
		[TestCase("hey@gmail.c(m")]
		[TestCase("hey@gmail.")]
		[TestCase("h()y@gmail.com")]
		public void WrongEmail_ShouldNotBeMatchedSuccessfully(string email)
		{
			var isSuccess = Regexes.EmailRegex.Match(email).Success;
			Assert.That(isSuccess, Is.False);
		}
	}
}
