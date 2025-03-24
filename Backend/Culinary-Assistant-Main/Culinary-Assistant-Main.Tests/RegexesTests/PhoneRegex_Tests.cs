using Culinary_Assistant.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.RegexesTests
{
	[TestFixture]
	public class PhoneRegex_Tests
	{
		[TestCase("+79345567374")]
		[TestCase("89345567374")]
		public void CorrectPhone_ShouldBeMatchedSuccessfully(string phone)
		{
			var isSuccess = Regexes.PhoneRegex.Match(phone).Success;
			Assert.That(isSuccess, Is.True);
		}

		[TestCase("79345567374")]
		[TestCase("+89345567374")]
		[TestCase("+7893455673")]
		[TestCase("+789345567345")]
		[TestCase("8992016937")]
		[TestCase("899201693701")]
		[TestCase("absolutewro")]
		[TestCase("$%%^^^#&@#$")]
		public void WrongPhone_ShouldNotBeMatchedSuccessfully(string phone)
		{
			var isSuccess = Regexes.PhoneRegex.Match(phone).Success;
			Assert.That(isSuccess, Is.False);
		}
	}
}
