using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Shared.Serializable
{
	public record CookingStep(int Step, string Title, string Description);
}
