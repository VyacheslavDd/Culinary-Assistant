using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Tests.ModelsTests
{
	[TestFixture]
	public class ReceiptModel_Tests
	{
		private Receipt _receipt;

		[SetUp]
		public void SetUp()
		{
			List<Ingredient> ingredients = [new Ingredient("Ingredient 1", 1, Measure.Gram)];
			List<CookingStep> steps = [new CookingStep(1, "Первый", "Chill")];
			List<FilePath> pictureUrls = [new FilePath("http://picture.com")];
			var receiptInDTO = new ReceiptInDTO("Receipt", "Description Receipt", [], Category.Soup, CookingDifficulty.Easy, 60, ingredients, steps, pictureUrls, Guid.Empty);
			_receipt = Receipt.Create(receiptInDTO).Value;
		}

		[Test]
		public void SetTitle_WorksCorrectly()
		{
			var res = _receipt.SetTitle("updated");
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(_receipt.Title.Value, Is.EqualTo("updated"));
			});
		}

		[Test]
		public void SetDescription_WorksCorrectly()
		{
			var res = _receipt.SetDescription("updated");
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(_receipt.Description.Value, Is.EqualTo("updated"));
			});
		}

		[Test]
		public void SetCategory_WorksCorrectly()
		{
			_receipt.SetCategory(Category.Dinner);
			Assert.That(_receipt.Category, Is.EqualTo(Category.Dinner));
		}

		[Test]
		public void SetTags_WorksCorrectly()
		{
			_receipt.SetTags([Tag.Vegetarian, Tag.Lenten]);
			Assert.That(_receipt.Tags, Is.EqualTo("0|1"));
		}

		[Test]
		public void SetCookingDifficulty_WorksCorrectly()
		{
			_receipt.SetCookingDifficulty(CookingDifficulty.Hard);
			Assert.That(_receipt.CookingDifficulty, Is.EqualTo(CookingDifficulty.Hard));
		}

		[Test]
		public void SetCookingTime_WorksCorrectly()
		{
			var res = _receipt.SetCookingTime(2);
			Assert.Multiple(() =>
			{
				Assert.That(res.IsSuccess, Is.True);
				Assert.That(_receipt.CookingTime, Is.EqualTo(2));
			});
		}

		[Test]
		public void SetUserId_WorksCorrectly()
		{
			var guid = new Guid();
			_receipt.SetUserId(guid);
			Assert.That(_receipt.UserId, Is.EqualTo(guid));
		}

		[Test]
		public void SetPictures_WorksCorrectly()
		{
			List<FilePath> pictures = [new FilePath("url1"), new FilePath("url2"), new FilePath("url3")];
			_receipt.SetPictures(pictures);
			Assert.Multiple(() =>
			{
				Assert.That(_receipt.MainPictureUrl, Is.EqualTo("url1"));
				Assert.That(_receipt.PicturesUrls, Is.EqualTo("[{\"Url\":\"url1\"},{\"Url\":\"url2\"},{\"Url\":\"url3\"}]"));
			});

		}

		[Test]
		public void SetIngredients_WorksCorrectly()
		{
			List<Ingredient> ingredients = [new Ingredient("water", 2, Measure.Liter)];
			_receipt.SetIngredients(ingredients);
			Assert.That(_receipt.Ingredients, Is.EqualTo("[{\"Name\":\"water\",\"NumericValue\":2,\"Measure\":2}]"));
		}

		[Test]
		public void SetCookingSteps_WorksCorrectly()
		{
			List<CookingStep> cookingSteps = [new CookingStep(1, "1", "1")];
			_receipt.SetCookingSteps(cookingSteps);
			Assert.That(_receipt.CookingSteps, Is.EqualTo("[{\"Step\":1,\"Title\":\"1\",\"Description\":\"1\"}]"));
		}

		[Test]
		public void SetNutrients_WorksCorrectly()
		{
			var nutrientsResult = _receipt.SetNutrients(200, 20, 23, 17);
			Assert.Multiple(() =>
			{
				Assert.That(nutrientsResult.IsSuccess, Is.True);
				Assert.That(_receipt.Nutrients.Calories, Is.EqualTo(200));
				Assert.That(_receipt.Nutrients.Proteins, Is.EqualTo(20));
				Assert.That(_receipt.Nutrients.Fats, Is.EqualTo(23));
				Assert.That(_receipt.Nutrients.Carbohydrates, Is.EqualTo(17));
			});
		}

		[Test]
		public void AddPopularity_WorksCorrectly()
		{
			var popularityBefore = _receipt.Popularity;
			_receipt.AddPopularity();
			Assert.Multiple(() =>
			{
				Assert.That(popularityBefore, Is.EqualTo(0));
				Assert.That(_receipt.Popularity, Is.EqualTo(1));
			});
		}

		[Test]
		public void SetRating_WorksCorrectly()
		{
			_receipt.SetRating(5.3);
			Assert.That(_receipt.Rating, Is.EqualTo(5.3));
		}

		[Test]
		public void CannotSet_LongTitle()
		{
			var result = _receipt.SetTitle("longtitle1longtitle1longtitle1longtitle1longtitle1longtitle1longtitle1longtitle1longtitle1longtitle1longtitle1");
			Assert.That(result.IsFailure, Is.True);
		}

		[Test]
		public void CannotSet_NegativeNutrients()
		{
			var negativeProteinsResult = _receipt.SetNutrients(200, -1, 10, 15);
			var negativeCaloriesResult = _receipt.SetNutrients(-1, 5, 10, 15);
			var negativeFatsResult = _receipt.SetNutrients(200, 5, -1, 15);
			var negativeCarbohydratesResult = _receipt.SetNutrients(200, 5, 10, -1);
			Assert.Multiple(() =>
			{
				Assert.That(negativeCaloriesResult.IsFailure, Is.True);
				Assert.That(negativeProteinsResult.IsFailure, Is.True);
				Assert.That(negativeFatsResult.IsFailure, Is.True);
				Assert.That(negativeCarbohydratesResult.IsFailure, Is.True);
			});
		}

		[Test]
		public void CannotSet_NegativeCookingTime()
		{
			var res = _receipt.SetCookingTime(-30);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public void CannotSet_IncorrectSteps()
		{
			var steps = new List<CookingStep>() { new(1, "1", "1"), new(3, "2", "2") };
			var res = _receipt.SetCookingSteps(steps);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public void CannotSet_EmptyIngredientsList()
		{
			var ingredients = new List<Ingredient>();
			var res = _receipt.SetIngredients(ingredients);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public void CannotSet_EmptyCookingStepsList()
		{
			var cookingSteps = new List<CookingStep>();
			var res = _receipt.SetCookingSteps(cookingSteps);
			Assert.That(res.IsFailure, Is.True);
		}

		[Test]
		public void CannotSet_EmptyPicturesList()
		{
			var pictures = new List<FilePath>();
			var res = _receipt.SetPictures(pictures);
			Assert.That(res.IsFailure, Is.True);
		}
	}
}
