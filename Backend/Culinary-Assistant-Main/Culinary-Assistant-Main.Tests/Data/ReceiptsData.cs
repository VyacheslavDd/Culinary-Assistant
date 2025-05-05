using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant_Main.Tests.Common;

namespace Culinary_Assistant_Main.Tests.Data
{
	public static class ReceiptsData
	{
		public static List<Receipt> Receipts =>
			[
				Receipt.Create(new ReceiptInDTO("Название", "Описание", [Tag.Vegetarian], Category.Breakfast, CookingDifficulty.Easy,
					50, [new Ingredient("Морковь", 3, Measure.Piece), new Ingredient("Свекла", 2, Measure.Piece)],
					[new CookingStep(1, "Первый", "Один"), new CookingStep(2, "Второй", "Два\nXD")],
					[new FilePath("https://placehold.co/600x400")], default)).Value,

				Receipt.Create(new ReceiptInDTO("Суп", "Обыкновенный суп", [Tag.Lenten], Category.Soup, CookingDifficulty.Medium,
					80, [new Ingredient("Картошка", 500, Measure.Gram), new Ingredient("Вода", 2, Measure.Liter)],
					[new CookingStep(1, "Во-первых", "Вскипятить воду"), new CookingStep(2, "Во-вторых", "Почистить картошку")],
					[new FilePath("https://placehold.co/800x400"), new FilePath("https://placehold.co/1020x580")], default)).Value,

				Receipt.Create(new ReceiptInDTO("Салат", "Вкусный салат", [Tag.Lenten], Category.Dinner, CookingDifficulty.Hard,
					60, [new Ingredient("Огурец", 3, Measure.Piece), new Ingredient("Помидор", 2, Measure.Piece)],
					[new CookingStep(1, "Во-первых", "Порезать огурец"), new CookingStep(2, "Шаг 2.", "Порезать помидор")],
					[new FilePath("https://placehold.co/1000x400")], default)).Value
			];
	}
}
