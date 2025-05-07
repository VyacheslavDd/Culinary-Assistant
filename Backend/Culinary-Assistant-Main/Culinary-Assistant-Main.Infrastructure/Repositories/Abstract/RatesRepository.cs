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
    public abstract class RatesRepository<T, TRated>(CulinaryAppContext culinaryAppContext, DbSet<T> dbSet) : IRatesRepository<T, TRated> where T : Rate<T, TRated>, new()
        where TRated : Core.Base.Entity<Guid>
    {
        private readonly CulinaryAppContext _culinaryAppContext = culinaryAppContext;
        private readonly DbSet<T> _dbSet = dbSet;

        public async Task AddAsync(T rate)
        {
            await _dbSet.AddAsync(rate);
            await SaveChangesAsync();
        }

        public IQueryable<T> GetAllRatesForEntity(Guid id)
        {
            return _dbSet.Where(rr => rr.EntityId == id);
        }

        public async Task<T?> GetAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(rr => rr.UserId == userId && rr.EntityId == id, cancellationToken);
        }

        public async Task SaveChangesAsync()
        {
            await _culinaryAppContext.SaveChangesAsync();
        }
    }
}
