using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.ReceiptRate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models
{
	public class ReceiptRate : Core.Base.Entity<Guid>
	{
		public Guid? UserId { get; private set; }
		public Guid ReceiptId { get; private set; }
		public int Rate { get; private set; }
		public User? User { get; private set; }
		public Receipt Receipt { get; private set; }

		public static Result<ReceiptRate> Create(ReceiptRateModelDTO receiptRateInDTO)
		{
			var rate = new ReceiptRate();
			var rateRes = rate.SetRate(receiptRateInDTO.Rate);
			if (rateRes.IsFailure) return Result.Failure<ReceiptRate>(rateRes.Error);
			rate.SetUserId(receiptRateInDTO.UserId);
			rate.SetReceiptId(receiptRateInDTO.ReceiptId);
			return Result.Success(rate);
		}

		public void SetUserId(Guid id)
		{
			UserId = id;
		}

		public void SetReceiptId(Guid id)
		{
			ReceiptId = id;
		}

		public Result SetRate(int rate)
		{
			if (rate < MiscellaneousConstants.MinReceiptRate || rate > MiscellaneousConstants.MaxReceiptRate)
				return Result.Failure($"Оценка рецепта должна быть значением от {MiscellaneousConstants.MinReceiptRate} до {MiscellaneousConstants.MaxReceiptRate}");
			Rate = rate;
			return Result.Success();
		}
	}
}
