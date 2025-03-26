﻿using Culinary_Assistant.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Shared.Serializable
{
	public record Ingredient(string Name, int NumericValue, Measure Measure);
}
