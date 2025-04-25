using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.ValueTypes
{
	public class Text : ValueObject
	{
		public string Value { get; private set; }

		protected override IEnumerable<object> GetEqualityComponents()
		{
			throw new NotImplementedException();
		}

		public static Result<Text> Create(string text, int maxLength)
		{
			if (string.IsNullOrWhiteSpace(text) || text.Length > maxLength)
			{
				return Result.Failure<Text>($"Текст пустой или превышает длину: {maxLength}");
			}
			return Result.Success(new Text() { Value =  text });
		}
	}
}
