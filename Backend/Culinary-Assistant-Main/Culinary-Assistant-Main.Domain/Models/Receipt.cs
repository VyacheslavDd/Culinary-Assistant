using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant.Core.ValueTypes;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using Culinary_Assistant_Main.Domain.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models
{
    public class Receipt : Core.Base.Entity<Guid>, IRateable
	{
		private readonly List<ReceiptCollection> _receiptCollections = [];
		private readonly List<ReceiptLike> _likes = [];
		private readonly List<ReceiptRate> _rates = [];
		private readonly List<Feedback> _feedbacks = [];

		public Text Title { get; private set; }
		public Text Description { get; private set; }
		public CookingDifficulty CookingDifficulty { get; private set; }
		public Category Category { get; private set; }
		public string Tags { get; private set; }
		public int CookingTime { get; private set; }
		public string Ingredients { get; private set; }
		public string CookingSteps { get; private set; }
		public Nutrients Nutrients { get; private set; }
		public int Popularity { get; private set; }
		public string MainPictureUrl { get; private set; }
		public string PicturesUrls { get; private set; }
		public double Rating { get; private set; }
		public Guid UserId { get; private set; }
		public User User { get; private set; }
		public DateTime UpdatedAt { get; private set; }
		public IReadOnlyCollection<ReceiptCollection> ReceiptCollections => _receiptCollections;
		public IReadOnlyCollection<ReceiptLike> Likes => _likes;
		public IReadOnlyCollection<ReceiptRate> Rates => _rates;
		public IReadOnlyCollection<Feedback> Feedbacks => _feedbacks;

		public static Result<Receipt> Create(ReceiptInDTO receiptInDTO)
		{
			var results = Miscellaneous.CreateResultList(6);
			var receipt = new Receipt();
			results[0] = receipt.SetTitle(receiptInDTO.Title);
			results[1] = receipt.SetDescription(receiptInDTO.Description);
			results[2] = receipt.SetCookingTime(receiptInDTO.CookingTime);
			results[3] = receipt.SetCookingSteps(receiptInDTO.CookingSteps);
			results[4] = receipt.SetIngredients(receiptInDTO.Ingredients);
			results[5] = receipt.SetPictures(receiptInDTO.PicturesUrls);
			if (results.Any(r => r.IsFailure))
				return Result.Failure<Receipt>(Miscellaneous.ResultFailureWithAllFailuresFromResultList(results).Error);
			receipt.SetCategory(receiptInDTO.Category);
			receipt.SetCookingDifficulty(receiptInDTO.CookingDifficulty);
			receipt.SetTags(receiptInDTO.Tags);
			receipt.SetUserId(receiptInDTO.UserId);
			receipt.ActualizeUpdatedAtField();
			return Result.Success(receipt);
		}

		public Result SetTitle(string title)
		{
			var titleResult = Text.Create(title, 100);
			if (titleResult.IsFailure) return Result.Failure(titleResult.Error);
			Title = titleResult.Value;
			return Result.Success();
		}

		public Result SetDescription(string description)
		{
			var descriptionResult = Text.Create(description, 1000);
			if (descriptionResult.IsFailure) return Result.Failure(descriptionResult.Error);
			Description = descriptionResult.Value;
			return Result.Success();
		}

		public void SetCookingDifficulty(CookingDifficulty cookingDifficulty)
		{
			CookingDifficulty = cookingDifficulty;
		}

		public Result SetCookingTime(int cookingTime)
		{
			if (cookingTime <= 0)
				return Result.Failure("Время приготовления должно быть от 1 минуты");
			CookingTime = cookingTime;
			return Result.Success();
		}

		public void SetCategory(Category category)
		{
			Category = category;
		}

		public void SetTags(List<Tag> tags)
		{
			Tags = string.Join(MiscellaneousConstants.ValuesSeparator, tags.Select(t => (int)t));
		}

		public Result SetIngredients(List<Ingredient> ingredients)
		{
			if (ingredients.Count == 0) return Result.Failure("Должны присутствовать ингредиенты");
			foreach (var ingredient in ingredients) { 
				if (string.IsNullOrWhiteSpace(ingredient.Name))
					return Result.Failure("Название ингредиента не может быть пустым");
				if (ingredient.NumericValue <= 0)
					return Result.Failure("Количество ингредиента должно быть больше 0");
			}
			Ingredients = JsonSerializer.Serialize(ingredients);
			return Result.Success();
		}

		public Result SetCookingSteps(List<CookingStep> cookingSteps)
		{
			if (cookingSteps.Count == 0) return Result.Failure("Последовательность шагов не может быть пустой");
			var currentStep = 1;
			foreach (var step in cookingSteps)
			{
				if (string.IsNullOrWhiteSpace(step.Description) || string.IsNullOrWhiteSpace(step.Title))
					return Result.Failure("Название и описание шага не могут быть пустыми");
				if (currentStep != step.Step)
					return Result.Failure($"Нарушена последовательность шагов приготовления. Ожидаемый пункт: {currentStep}. Фактический: {step.Step}");
				currentStep++;
			}
			CookingSteps = JsonSerializer.Serialize(cookingSteps);
			return Result.Success();
		}

		public Result SetPictures(List<FilePath> picturesUrls)
		{
			if (picturesUrls.Count == 0) return Result.Failure("Должна присутствовать хотя бы обложка у рецепта");
			MainPictureUrl = picturesUrls[0].Url;
			PicturesUrls = JsonSerializer.Serialize(picturesUrls);
			return Result.Success();
		}

		public void SetUserId(Guid userId)
		{
			UserId = userId;
		}

		public Result SetNutrients(double calories, double proteins, double fats, double carbohydrates)
		{
			var nutrientsResult =  Nutrients.Create(calories, proteins, fats, carbohydrates);
			if (nutrientsResult.IsSuccess)
			{
				Nutrients = nutrientsResult.Value;
				return Result.Success();
			}
			return Result.Failure(nutrientsResult.Error);
		}

		public void AddPopularity()
		{
			Popularity += 1;
		}

		public void SetRating(double rating)
		{
			Rating = Math.Round(rating, MiscellaneousConstants.RoundRatingToDigits);
		}

		/// <summary>
		/// Установить Id рецепта. Лучше не использовать, необходим только для тестирования!
		/// </summary>
		public void SetReceiptId(Guid id)
		{
			SetId(id);
		}

		public void ActualizeUpdatedAtField()
		{
			UpdatedAt = DateTime.UtcNow;
		}
	}

}
