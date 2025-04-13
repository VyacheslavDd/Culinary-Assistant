using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Utils
{
	public static class Regexes
	{
		public static Regex PhoneRegex => new("^(\\+7|8)[0-9]{10}$");
		public static Regex LoginRegex => new("^[a-zA-Z][A-Za-z0-9_]{0,23}[A-Za-z0-9]$");
		public static Regex PasswordRegex => new(@"^[\w!#$%&'*+\-/=?^`{|}~() ]{8,25}$");
		public static Regex EmailRegex => new("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
	}
}
