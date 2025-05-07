using Core.Base.Interfaces;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.DTO.Like;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Likes
{
	public interface ILikesService<T, TLiked> where T: Like<TLiked> where TLiked : Core.Base.Entity<Guid>
	{
		Task ApplyLikesInfoForUserAsync<TDTO>(ClaimsPrincipal user, List<TDTO> entities) where TDTO : IFavouritedDTO;
		Task ApplyLikeInfoForUserAsync<TDTO>(ClaimsPrincipal user, TDTO entity) where TDTO : IFavouritedDTO;
		Task<Result<List<TLiked>>> GetAllLikedEntitiesForUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
		Task<Result<Guid>> AddAsync(LikeInDTO likeInDTO);
		Task<Result> RemoveAsync(Guid userId, Guid entityId);
		Task<T?> GetAsync(Guid userId, Guid entityId, CancellationToken cancellationToken = default);
	}
}
