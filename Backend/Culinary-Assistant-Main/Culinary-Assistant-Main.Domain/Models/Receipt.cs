using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models
{
	public class Receipt : Core.Base.Entity<Guid>
	{
		public string Title { get; private set; }
		public string Description { get; private set; }
		public CookingDifficulty CookingDifficulty { get; private set; }
		public Category Category { get; private set; }
		public string Tags { get; private set; }
		public int CookingTime { get; private set; }
		public string Ingredients { get; private set; }
		public string CookingSteps { get; private set; }
		public int Calories { get; private set; }
		public int Proteins { get; private set; }
		public int Fats { get; private set; }
		public int Carbohydrates { get; private set; }
		public int Popularity { get; private set; }
		public string MainPictureUrl { get; private set; }
		public string PicturesUrls { get; private set; }
		public Guid UserId { get; private set; }
		public User User { get; private set; }

		public static Result<Receipt> Create(ReceiptInDTO receiptInDTO)
		{
			var receipt = new Receipt();
			receipt.SetTitle(receiptInDTO.Title);
			receipt.SetDescription(receiptInDTO.Description);
			receipt.SetCategory(receiptInDTO.Category);
			receipt.SetCookingDifficulty(receiptInDTO.CookingDifficulty);
			receipt.SetCookingTime(receiptInDTO.CookingTime);
			receipt.SetCookingSteps(receiptInDTO.CookingSteps);
			receipt.SetIngredients(receiptInDTO.Ingredients);
			receipt.SetTags(receiptInDTO.Tags);
			receipt.SetPictures(receiptInDTO.PicturesUrls);
			receipt.SetUserId(receiptInDTO.UserId);
			return Result.Success(receipt);
		}

		public void SetTitle(string title)
		{
			Title = title;
		}

		public void SetDescription(string description)
		{
			Description = description;
		}

		public void SetCookingDifficulty(CookingDifficulty cookingDifficulty)
		{
			CookingDifficulty = cookingDifficulty;
		}

		public void SetCookingTime(int cookingTime)
		{
			CookingTime = cookingTime;
		}

		public void SetCategory(Category category)
		{
			Category = category;
		}

		public void SetTags(List<Tag> tags)
		{
			Tags = string.Join(MiscellaneousConstants.ValuesSeparator, tags.Select(t => (int)t));
		}

		public void SetIngredients(List<Ingredient> ingredients)
		{
			Ingredients = JsonSerializer.Serialize(ingredients);
		}

		public void SetCookingSteps(List<CookingStep> cookingSteps)
		{
			CookingSteps = JsonSerializer.Serialize(cookingSteps);
		}

		public void SetPictures(List<PictureUrl> picturesUrls)
		{
			MainPictureUrl = picturesUrls[0].Url;
			PicturesUrls = JsonSerializer.Serialize(picturesUrls);
		}

		public void SetUserId(Guid userId)
		{
			UserId = userId;
		}
	}

}
