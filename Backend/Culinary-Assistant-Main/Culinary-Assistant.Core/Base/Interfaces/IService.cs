﻿using CSharpFunctionalExtensions;
using System.Linq.Expressions;

namespace Core.Base.Interfaces
{
	public interface IService<T, TCreateDTO, TUpdateDTO>
	{
		Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
		Task<T?> GetByGuidAsync(Guid id, CancellationToken cancellationToken = default);
		abstract Task<Result<Guid>> CreateAsync(TCreateDTO entityCreateRequest, bool autoSave=true);
		Task SaveChangesAsync();
		Task<Result> NotBulkUpdateAsync(Guid entityId, TUpdateDTO updateRequest);
		Task<Result> NotBulkDeleteAsync(Guid entityId);
		Task<Result<string>> BulkDeleteAsync(Guid entityId);
	}
}
