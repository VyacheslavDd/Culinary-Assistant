﻿using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base.Interfaces
{
	public interface IRepository<T>
	{
		IQueryable<T> GetAll();
		Task<T?> GetBySelectorAsync(Expression<Func<T, bool>> selector, CancellationToken cancellationToken = default);
		IQueryable<T> GetAllBySelectorAsync(Expression<Func<T, bool>> selector, CancellationToken cancellationToken = default);
		IQueryable<T> GetAllWithInclude<TProperty>(Expression<Func<T, TProperty>> includeExpression);
		Task<Guid> AddAsync(T entity, bool autoSave=true);
		Task<int> BulkDeleteAsync(Expression<Func<T, bool>> selector);
		Task<Result> NotBulkDeleteAsync(Expression<Func<T, bool>> selector);
		Task SaveChangesAsync();
	}
}
