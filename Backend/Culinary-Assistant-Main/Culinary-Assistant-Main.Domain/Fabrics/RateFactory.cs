using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.ReceiptRate;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Fabrics
{
	public static class RateFactory
	{
		public static Result<T> Create<T, TRated>(RateModelDTO rateModelDTO) where T: Rate<T, TRated>, new() where TRated: Core.Base.Entity<Guid>
		{
			var rate = new T();
			var rateRes = rate.SetRate(rateModelDTO.Rate);
			if (rateRes.IsFailure) return Result.Failure<T>(rateRes.Error);
			rate.SetUserId(rateModelDTO.UserId);
			rate.SetEntityId(rateModelDTO.EntityId);
			return Result.Success(rate);
		}
	}
}
