using AutoMapper.Internal;
using Core.Base.Interfaces;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.DTO.Like;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Fabrics;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using Culinary_Assistant_Main.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Likes.Abstract
{
	public abstract class LikesService<T, TLiked>(ILikesRepository<T, TLiked> likesRepository, IUsersRepository usersRepository)
		: ILikesService<T, TLiked> where T : Like<TLiked>, new() where TLiked : Core.Base.Entity<Guid>
	{
		protected readonly ILikesRepository<T, TLiked> _repository = likesRepository;
		protected readonly IUsersRepository _usersRepository = usersRepository;

		public abstract Task<Result<Guid>> AddAsync(LikeInDTO likeInDTO);

		protected async Task<Result<Guid>> AddAsync(LikeInDTO likeInDTO, IRepository<TLiked> repository, Action<TLiked> onLike)
		{
			var user = await _usersRepository.GetBySelectorAsync(u => u.Id == likeInDTO.UserId);
			if (user == null) return Result.Failure<Guid>("Несуществующий пользователь не может поставить лайк");
			var entity = await repository.GetBySelectorAsync(r => r.Id == likeInDTO.EntityId);
			if (entity == null) return Result.Failure<Guid>("Нельзя поставить лайк на несуществующую сущность");
			var existingLike = await GetAsync(likeInDTO.UserId, likeInDTO.EntityId);
			if (existingLike != null) return Result.Failure<Guid>("Лайк уже выставлен на сущность");
			var like = LikeFactory.Create<T, TLiked>(likeInDTO);
			var guid = await _repository.AddAsync(like.Value);
			onLike(entity);
			await repository.SaveChangesAsync();
			return Result.Success(guid);
		}

		public async Task<T?> GetAsync(Guid userId, Guid entityId, CancellationToken cancellationToken = default)
		{
			return await _repository.GetByUserAndEntityIdsAsync(userId, entityId, cancellationToken);
		}

		public async Task ApplyLikesInfoForUserAsync<TDTO>(ClaimsPrincipal user, List<TDTO> entities) where TDTO : ILikedDTO
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(user);
			if (userId == Guid.Empty) return;
			var foundLikedEntities = await _repository.GetAll().Where(l => l.UserId == userId).Select(e => e.LikedEntityId).ToListAsync();
			var foundLikedEntitiesHashSet = new HashSet<Guid>(foundLikedEntities);
			entities.ForEach(e =>
			{
				if (foundLikedEntities.Contains(e.Id)) e.IsLiked = true;
			});
		}

		public async Task ApplyLikeInfoForUserAsync<TDTO>(ClaimsPrincipal user, TDTO entity) where TDTO : ILikedDTO
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(user);
			if (userId == Guid.Empty) return;
			var likedEntity = await GetAsync(userId, entity.Id);
			if (likedEntity != null) entity.IsLiked = true;
		}
	}
}
