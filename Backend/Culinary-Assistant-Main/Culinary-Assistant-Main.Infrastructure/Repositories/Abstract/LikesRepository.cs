using Culinary_Assistant_Main.Domain.Models.Abstract;
using Culinary_Assistant_Main.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Repositories.Abstract
{
	public abstract class LikesRepository<T, TLiked>(CulinaryAppContext context, DbSet<T> dbSet) : ILikesRepository<T, TLiked> where T: Like<TLiked> where TLiked : Core.Base.Entity<Guid>
	{
		protected readonly CulinaryAppContext _context = context;
		protected readonly DbSet<T> _dbSet = dbSet;

		public async Task<Guid> AddAsync(T like)
		{
			var res = await _dbSet.AddAsync(like);
			await _context.SaveChangesAsync();
			var guid = res.Entity.Id;
			return guid;
		}

		public async Task<T?> GetByUserAndEntityIdsAsync(Guid userId, Guid entityId, CancellationToken cancellationToken = default)
		{
			return await _dbSet.FirstOrDefaultAsync(l => l.UserId == userId && l.LikedEntityId == entityId, cancellationToken);
		}
	}
}
