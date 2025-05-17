using Core.Base.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Minio.DataModel.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base
{
	public abstract class BaseRepository<T>(DbContext dbContext, DbSet<T> dbSet) : IRepository<T> where T: class
	{
		protected readonly DbContext _dbContext = dbContext;
		protected readonly DbSet<T> _dbSet = dbSet;

		public virtual IQueryable<T> GetAll()
		{
			return _dbSet;
		}

		public virtual async Task<int> GetEntitiesCountAsync(CancellationToken cancellationToken = default)
		{
			return await _dbSet.CountAsync(cancellationToken);
		}

		public virtual async Task<T?> GetBySelectorAsync(Expression<Func<T, bool>> selector, CancellationToken cancellationToken = default)
		{
			return await _dbSet.Where(selector).FirstOrDefaultAsync(cancellationToken);
		}

		public virtual IQueryable<T> GetAllBySelector(Expression<Func<T, bool>> selector, CancellationToken cancellationToken = default)
		{
			return _dbSet.Where(selector);
		}

		public virtual IQueryable<T> GetAllWithInclude<TProperty>(Expression<Func<T, TProperty>> includeExpression)
		{
			return _dbSet.Include(includeExpression);
		}

		public virtual async Task<Guid> AddAsync(T entity, bool autoSave=true)
		{
			var createdEntity = await _dbSet.AddAsync(entity);
			if (autoSave)
				await SaveChangesAsync();
			return (Guid)createdEntity.Property("Id").CurrentValue;
		}

		public virtual async Task<int> BulkDeleteAsync(Expression<Func<T, bool>> selector)
		{
			return await _dbSet.Where(selector).ExecuteDeleteAsync();
		}

		public virtual async Task<Result<string>> NotBulkDeleteAsync(Expression<Func<T, bool>> selector)
		{
			var entities = await _dbSet.Where(selector).ToListAsync();
			if (entities.Count == 0) return Result.Success("Удалено строк: 0");
			_dbSet.RemoveRange(entities);
			await SaveChangesAsync();
			return Result.Success($"Удалено строк: {entities.Count}");
		}

		public virtual async Task SaveChangesAsync()
		{
			await _dbContext.SaveChangesAsync();
		}

		public async Task LoadReferenceAsync<TProperty>(T entity, Expression<Func<T, TProperty?>> referenceExpression) where TProperty: class
		{
			await _dbSet.Entry(entity).Reference(referenceExpression).LoadAsync();
		}

		public async Task LoadCollectionAsync<TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> collectionExpression) where TProperty : class
		{
			await _dbSet.Entry(entity).Collection(collectionExpression).LoadAsync();
		}
	}
}
