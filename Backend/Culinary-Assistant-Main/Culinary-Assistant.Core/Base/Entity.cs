using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base
{
	public abstract class Entity<T> where T : struct
	{
		public T Id { get; set; }
	}
}
