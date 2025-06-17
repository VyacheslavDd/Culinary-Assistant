using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.ValueTypes
{
	public class Nutrients : ValueObject
	{
		public double Calories { get; private set; }
		public double Proteins { get; private set; }
		public double Fats { get; private set; }
		public double Carbohydrates { get; private set; }

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Calories;
			yield return Proteins;
			yield return Fats;
			yield return Carbohydrates;
		}

		public static Result<Nutrients> Create(double calories, double proteins, double fats, double carbohydrates)
		{
			List<double> nutrients = [calories, proteins, fats, carbohydrates];
			if (nutrients.Any(n => n < 0))
			{
				return Result.Failure<Nutrients>("Ни один нутриент не может быть меньше 0");
			}
			var roundedNutrients = nutrients.Select(n => Math.Round(n, 1)).ToList();
			var nutrientsObj = new Nutrients()
			{
				Calories = roundedNutrients[0],
				Proteins = roundedNutrients[1],
				Fats = roundedNutrients[2],
				Carbohydrates = roundedNutrients[3]
			};
			return Result.Success(nutrientsObj);
		}
	}
}
