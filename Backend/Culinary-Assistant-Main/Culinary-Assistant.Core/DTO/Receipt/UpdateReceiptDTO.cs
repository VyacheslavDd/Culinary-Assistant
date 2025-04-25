using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.Receipt
{
	public record UpdateReceiptDTO(string? Title = null, string? Description = null, List<Tag>? Tags = null, Category? Category = null,
		CookingDifficulty? CookingDifficulty = null, int? CookingTime = null, List<Ingredient>? Ingredients = null,
		List<CookingStep>? CookingSteps = null, List<FilePath>? PicturesUrls = null);
}
