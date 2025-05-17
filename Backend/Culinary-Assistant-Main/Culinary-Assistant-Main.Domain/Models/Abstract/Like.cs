using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Like;
using Culinary_Assistant_Main.Domain.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models.Abstract
{
    public abstract class Like<TLiked> : Core.Base.Entity<Guid> where TLiked : Core.Base.Entity<Guid>
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public Guid LikedEntityId { get; private set; }
        public TLiked Entity { get; private set; }

        public static Result<T> Create<T>(LikeInDTO likeInDTO) where T: Like<TLiked>, new()
        {
            var like = new T();
            like.SetUserId(likeInDTO.UserId);
            like.SetEntityId(likeInDTO.EntityId);
            return Result.Success(like);
        }

        public virtual void SetUserId(Guid userId)
        {
            UserId = userId;
        }

        public virtual void SetEntityId(Guid entityId)
        {
            LikedEntityId = entityId;
        }
    }
}
