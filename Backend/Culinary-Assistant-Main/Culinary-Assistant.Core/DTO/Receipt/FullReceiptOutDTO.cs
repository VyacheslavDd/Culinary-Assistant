using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.Receipt
{
	public class FullReceiptOutDTO : IFavouritedDTO
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public List<Ingredient> Ingredients { get; set; }
		public List<CookingStep> CookingSteps { get; set; }
		public double Calories { get; set; }
		public int CookingTime { get; set; }
		public double Proteins { get; set; }
		public double Fats { get; set; }
		public double Carbohydrates { get; set; }
		public List<Tag> Tags { get; set; }
		public Category Category { get; set; }
		public CookingDifficulty CookingDifficulty { get; set; }
		public int Popularity { get; set; }
		public double Rating { get; set; }
		public bool IsFavourited { get; set; }
		public string MainPictureUrl { get; set; }
		public List<FilePath> PicturesUrls { get; set; }
		public ShortUserOutDTO User { get; set; }
	}
}
