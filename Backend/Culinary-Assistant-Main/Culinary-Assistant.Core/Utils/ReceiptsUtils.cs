using Culinary_Assistant.Core.Base.Data;
using Culinary_Assistant.Core.DTO.Nutrients;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Utils
{
	public static class ReceiptsUtils
	{
		public static double GetIngredientWeight(Ingredient ingredient)
		{
			var ingredientWeight = ingredient.Measure switch
			{
				Measure.Glass => WeightsMiscellaneousData.GlassWeight * ingredient.NumericValue,
				Measure.Pinch => WeightsMiscellaneousData.PinchWeight * ingredient.NumericValue,
				Measure.Gram => ingredient.NumericValue,
				Measure.Milliliter => ingredient.NumericValue,
				Measure.Kilogram => 1000 * ingredient.NumericValue,
				Measure.Liter => 1000 * ingredient.NumericValue,
				Measure.Piece => GetWeightDataFromDictionary(ingredient.Name, ingredient.NumericValue, PieceWeightsData.PieceWeights),
				Measure.Teaspoon => GetWeightDataFromDictionary(ingredient.Name, ingredient.NumericValue, TeaSpoonWeightsData.TeaSpoonWeights),
				Measure.Tablespoon => GetWeightDataFromDictionary(ingredient.Name, ingredient.NumericValue, TableSpoonWeightsData.TableSpoonWeights),
				_ => 0
			};
			return ingredientWeight;
		}

		private static double GetWeightDataFromDictionary(string ingredientName, double numericValue, Dictionary<string, double> weightsData)
		{
			var foundWeight = weightsData.TryGetValue(ingredientName.ToLower(), out var weight);
			if (!foundWeight) return 0;
			return weight * numericValue;
		}
	}
}
