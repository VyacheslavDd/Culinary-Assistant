using Culinary_Assistant_Main.Domain.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Repositories
{
    public interface IRatesRepository<T, TRated> where T : Rate<T, TRated>, new() where TRated: Core.Base.Entity<Guid>
	{
		Task<T?> GetAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
		IQueryable<T> GetAllRatesForEntity(Guid id);
		Task AddAsync(T rate);
		Task SaveChangesAsync();
	}
}
