using Culinary_Assistant_Main.Domain.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Repositories
{
	public interface ILikesRepository<T, TLiked> where T: Like<TLiked> where TLiked : Core.Base.Entity<Guid>
	{
		Task<Guid> AddAsync(T like);
		Task<T?> GetByUserAndEntityIdsAsync(Guid userId, Guid entityId, CancellationToken cancellationToken = default);
	}
}
