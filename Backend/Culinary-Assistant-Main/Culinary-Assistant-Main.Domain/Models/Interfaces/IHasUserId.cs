﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models.Interfaces
{
	public interface IHasUserId
	{
		Guid UserId { get; }
	}
}
