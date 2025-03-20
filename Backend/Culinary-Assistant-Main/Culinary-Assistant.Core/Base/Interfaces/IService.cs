using CSharpFunctionalExtensions;
using System.Linq.Expressions;

namespace Core.Base.Interfaces
{
	public interface IService<T, TDTO>
	{
		Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
		Task<T?> GetByGuidAsync(Guid id, CancellationToken cancellationToken = default);
		abstract Task<Result<Guid>> CreateAsync(TDTO entity, bool autoSave=true);
		Task SaveChangesAsync();
		Task<Result> NotBulkUpdateAsync(Guid entityId, TDTO updateRequest);
		Task<Result> NotBulkDeleteAsync(Guid entityId);
		Task<Result<string>> BulkDeleteAsync(Guid entityId);
	}
}
