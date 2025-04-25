using Culinary_Assistant.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.RegexesTests
{
	[TestFixture]
	public class LoginRegex_Tests
	{
		[TestCase("he")]
		[TestCase("hello_there")]
		[TestCase("hellotheremightbegood_log")]
		[TestCase("hELLO_NAME")]
		[TestCase("he344NAME")]
		public void CorrectLogin_ShouldBeMatchedSuccessfully(string login)
		{
			var isSuccess = Regexes.LoginRegex.Match(login).Success;
			Assert.That(isSuccess, Is.True);
		}

		[TestCase("h")]
		[TestCase("heLLO_")]
		[TestCase("8hey")]
		[TestCase("_WRONG")]
		[TestCase("hellotheremightbegood_login")]
		public void WrongLogin_ShouldNotBeMatchedSuccessfully(string login)
		{
			var isSuccess = Regexes.LoginRegex.Match(login).Success;
			Assert.That(isSuccess, Is.False);
		}
	}
}
