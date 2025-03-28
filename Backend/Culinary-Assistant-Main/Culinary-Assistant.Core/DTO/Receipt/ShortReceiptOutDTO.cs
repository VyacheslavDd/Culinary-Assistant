﻿using Culinary_Assistant.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.Receipt
{
	public class ShortReceiptOutDTO
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int Calories { get; set; }
		public int CookingTime { get; set; }
		public List<Tag> Tags { get; set; }
		public Category Category { get; set; }
		public CookingDifficulty CookingDifficulty { get; set; }
		public int Popularity { get; set; }
		public string MainPictureUrl { get; set; }
	}
}
