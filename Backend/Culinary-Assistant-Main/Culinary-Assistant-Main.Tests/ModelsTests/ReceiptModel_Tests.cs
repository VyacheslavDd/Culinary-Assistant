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
			List<CookingStep> steps = [new CookingStep(1, "Chill")];
			List<PictureUrl> pictureUrls = [new PictureUrl("http://picture.com")];
			var receiptInDTO = new ReceiptInDTO("Receipt", "Description Receipt", [], Category.Soups, CookingDifficulty.Easy, 60, ingredients, steps, pictureUrls, Guid.Empty);
			_receipt = Receipt.Create(receiptInDTO).Value;
		}

		[Test]
		public void SetTitle_WorksCorrectly()
		{
			_receipt.SetTitle("updated");
			Assert.That(_receipt.Title, Is.EqualTo("updated"));
		}

		[Test]
		public void SetDescription_WorksCorrectly()
		{
			_receipt.SetDescription("updated");
			Assert.That(_receipt.Description, Is.EqualTo("updated"));
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
			_receipt.SetTags([Tag.Vegetarian, Tag.Lean]);
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
			_receipt.SetCookingTime(2);
			Assert.That(_receipt.CookingTime, Is.EqualTo(2));
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
			List<PictureUrl> pictures = [new PictureUrl("url1"), new PictureUrl("url2"), new PictureUrl("url3")];
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
			Assert.That(_receipt.Ingredients, Is.EqualTo("[{\"Name\":\"water\",\"NumericValue\":2,\"Measure\":3}]"));
		}

		[Test]
		public void SetCookingSteps_WorksCorrectly()
		{
			List<CookingStep> cookingSteps = [new CookingStep(1, "1")];
			_receipt.SetCookingSteps(cookingSteps);
			Assert.That(_receipt.CookingSteps, Is.EqualTo("[{\"Step\":1,\"Description\":\"1\"}]"));
		}
	}
}
