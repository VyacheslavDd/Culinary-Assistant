using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.ServicesResponses;
using System.Linq.Expressions;

namespace Core.Base.Interfaces
{
	public interface IService<T, TCreateDTO, TUpdateDTO>
	{
		IQueryable<T> GetAll();
		Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
		EntitiesResponseWithCountAndPages<T> ApplyPaginationToEntities(List<T> entities, IPaginationFilter paginationFilter);
		Task<T?> GetByGuidAsync(Guid id, CancellationToken cancellationToken = default);
		abstract Task<Result<Guid>> CreateAsync(TCreateDTO entityCreateRequest, bool autoSave=true);
		Task SaveChangesAsync();
		Task<Result> NotBulkUpdateAsync(Guid entityId, TUpdateDTO updateRequest);
		Task<Result<string>> NotBulkDeleteAsync(Guid entityId);
		Task<Result<string>> BulkDeleteAsync(Guid entityId);
	}
}
