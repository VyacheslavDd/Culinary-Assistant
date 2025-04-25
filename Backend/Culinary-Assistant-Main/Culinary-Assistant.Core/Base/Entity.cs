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
		public T Id { get; private set; }

		public override string ToString()
		{
			return $"{typeof(T).Name}: {Id}";
		}

		protected void SetId(T value)
		{
			Id = value;
		}
	}
}
