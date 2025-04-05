using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.Receipt
{
	public record UpdateReceiptDTO(string? Title, string? Description, List<Tag>? Tags, Category? Category, CookingDifficulty? CookingDifficulty, int? CookingTime,
		List<Ingredient>? Ingredients, List<CookingStep>? CookingSteps, List<PictureUrl>? PicturesUrls);
}
