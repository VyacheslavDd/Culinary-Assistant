﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO
{
	public interface IUpdatePasswordDTO
	{
		string NewPassword { get; }
		string NewPasswordConfirmation { get; }
	}
}
