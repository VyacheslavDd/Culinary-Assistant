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

		protected async Task<Result<Guid>> AddAsync(LikeInDTO likeInDTO, IRepository<TLiked> repository)
		{
			var user = await _usersRepository.GetBySelectorAsync(u => u.Id == likeInDTO.UserId);
			if (user == null) return Result.Failure<Guid>("Несуществующий пользователь");
			var entity = await repository.GetBySelectorAsync(r => r.Id == likeInDTO.EntityId);
			if (entity == null) return Result.Failure<Guid>("Несуществующая сущность");
			var existingLike = await GetAsync(likeInDTO.UserId, likeInDTO.EntityId);
			if (existingLike != null) return Result.Failure<Guid>("Сущность уже в избранном");
			var like = LikeFactory.Create<T, TLiked>(likeInDTO);
			var guid = await _repository.AddAsync(like.Value);
			await repository.SaveChangesAsync();
			return Result.Success(guid);
		}

		public async Task<T?> GetAsync(Guid userId, Guid entityId, CancellationToken cancellationToken = default)
		{
			return await _repository.GetByUserAndEntityIdsAsync(userId, entityId, cancellationToken);
		}

		public async Task ApplyLikesInfoForUserAsync<TDTO>(ClaimsPrincipal user, List<TDTO> entities) where TDTO : IFavouritedDTO
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(user);
			if (userId == Guid.Empty) return;
			var foundLikedEntities = await _repository.GetAll().Where(l => l.UserId == userId).Select(e => e.LikedEntityId).ToListAsync();
			var foundLikedEntitiesHashSet = new HashSet<Guid>(foundLikedEntities);
			entities.ForEach(e =>
			{
				if (foundLikedEntities.Contains(e.Id)) e.IsFavourited = true;
			});
		}

		public async Task ApplyLikeInfoForUserAsync<TDTO>(ClaimsPrincipal user, TDTO entity) where TDTO : IFavouritedDTO
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(user);
			if (userId == Guid.Empty) return;
			var likedEntity = await GetAsync(userId, entity.Id);
			if (likedEntity != null) entity.IsFavourited = true;
		}

		public async Task<Result> RemoveAsync(Guid userId, Guid entityId)
		{
			var user = await _usersRepository.GetBySelectorAsync(u => u.Id == userId);
			if (user == null) return Result.Failure("Несуществующий пользователь");
			await _repository.RemoveAsync(userId, entityId);
			return Result.Success();
		}

		public async Task<Result<List<TLiked>>> GetAllLikedEntitiesForUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
		{
			var userId = Miscellaneous.RetrieveUserIdFromHttpContextUser(user);
			if (userId == Guid.Empty) return Result.Failure<List<TLiked>>("Нельзя получить избранные сущности для несуществующего пользователя");
			var likedEntities = await _repository
				.GetAll()
				.Where(e => e.UserId == userId)
				.Select(e => e.Entity)
				.ToListAsync(cancellationToken);
			return Result.Success(likedEntities);
		}
	}
}
