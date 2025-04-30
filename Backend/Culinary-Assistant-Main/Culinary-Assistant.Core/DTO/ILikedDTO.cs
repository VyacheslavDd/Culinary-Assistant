using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO
{
	public interface ILikedDTO
	{
		Guid Id { get; set; }
		bool IsLiked { get; set; }
	}
}
