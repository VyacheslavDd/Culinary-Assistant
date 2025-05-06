using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.ReceiptRate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models.Abstract
{
    public abstract class Rate<T, TRated> : Core.Base.Entity<Guid> where T: Rate<T, TRated>, new()
        where TRated: Core.Base.Entity<Guid>
    {
        public Guid? UserId { get; private set; }
        public Guid EntityId { get; private set; }
        public int Rating { get; private set; }
        public User? User { get; private set; }
        public TRated Entity { get; private set; }

        public void SetUserId(Guid id)
        {
            UserId = id;
        }

        public void SetEntityId(Guid id)
        {
            EntityId = id;
        }

        public Result SetRate(int rate)
        {
            if (rate < MiscellaneousConstants.MinReceiptRate || rate > MiscellaneousConstants.MaxReceiptRate)
                return Result.Failure($"Оценка рецепта должна быть значением от {MiscellaneousConstants.MinReceiptRate} до {MiscellaneousConstants.MaxReceiptRate}");
            Rating = rate;
            return Result.Success();
        }
    }
}
