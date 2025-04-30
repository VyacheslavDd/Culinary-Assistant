using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Like;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Fabrics
{
	public static class LikeFactory
	{
		public static Result<T> Create<T, TLiked>(LikeInDTO likeInDTO) where T: Like<TLiked>, new() where TLiked: Core.Base.Entity<Guid>
		{
			var like = new T();
			like.SetUserId(likeInDTO.UserId);
			like.SetEntityId(likeInDTO.EntityId);
			return Result.Success(like);
		}
	}
}
