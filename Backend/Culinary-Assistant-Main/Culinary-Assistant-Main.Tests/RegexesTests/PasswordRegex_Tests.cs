using Culinary_Assistant.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.RegexesTests
{
	[TestFixture]
	public class PasswordRegex_Tests
	{
		[TestCase("hellothere")]
		[TestCase("rightint")]
		[TestCase("toolongidktoolongidkidkto")]
		[TestCase("to00many546numbe34s")]
		[TestCase("_%^&*^passH94F")]
		[TestCase("br(){ckets}")]
		[TestCase("CaMeLG00-D TOO")]
		public void CorrectPasswords_DoMatch(string password)
		{
			var res = Regexes.PasswordRegex.Match(password);
			Assert.That(res.Success, Is.True);
		}

		[TestCase("short")]
		[TestCase("toolongidktoolongidkidktol")]
		[TestCase("WILLWORK?№№№")]
		[TestCase(" الأيام هي: الأحد، الاثنين، الثلاثاء،")]
		public void WrongPassword_DoNotMatch(string password)
		{
			var res = Regexes.PasswordRegex.Match(password);
			Assert.That(res.Success, Is.False);
		}
	}
}
