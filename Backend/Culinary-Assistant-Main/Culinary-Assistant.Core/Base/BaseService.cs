using Core.Base.Interfaces;
using Core.Extensions;
using System.Reflection;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.Enums;

namespace Core.Base
{
	public abstract class BaseService<T, TCreateDTO, TUpdateDTO>(IRepository<T> repository, ILogger logger) : IService<T, TCreateDTO, TUpdateDTO> where T : Entity<Guid>
	{
		private readonly Func<Guid, Expression<Func<T, bool>>> _idSelectorExpression = (Guid id) => e => e.Id == id;
		protected readonly IRepository<T> _repository = repository;
		protected readonly ILogger _logger = logger;
		protected readonly string _entityTypeName = typeof(T).Name;

		public IQueryable<T> GetAll()
		{
			return _repository.GetAll();
		}

		public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _repository.GetAll().ToListAsync(cancellationToken);
		}

		public abstract Task<Result<Guid>> CreateAsync(TCreateDTO entityCreateRequest, bool autoSave = true);

		public virtual async Task SaveChangesAsync()
		{
			await _repository.SaveChangesAsync();
		}

		public virtual async Task<Result> NotBulkUpdateAsync(Guid entityId, TUpdateDTO updateRequest)
		{
			await SaveChangesAsync();
			return Result.Success();
		}

		public virtual async Task<Result<string>> NotBulkDeleteAsync(Guid entityId)
		{
			var response = await _repository.NotBulkDeleteAsync(_idSelectorExpression(entityId));
			return response;
		}

		public virtual async Task<T?> GetByGuidAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await _repository.GetBySelectorAsync(_idSelectorExpression(id), cancellationToken);
		}

		public virtual async Task<Result<string>> BulkDeleteAsync(Guid entityId)
		{
			var deletedRowsCount = await _repository.BulkDeleteAsync(_idSelectorExpression(entityId));
			_logger.LogEntityDeletion(_entityTypeName, entityId);
			return Result.Success($"Удалено строк: {deletedRowsCount}");
		}

		protected async Task<Result<Guid>> AddToRepositoryAsync(Result<T> creationInfo, bool autoSave=true)
		{
			if (!creationInfo.IsSuccess) return Result.Failure<Guid>(creationInfo.Error);
			var answer = await _repository.AddAsync(creationInfo.Value, autoSave);
			_logger.LogEntityCreation(_entityTypeName, creationInfo.Value);
			return Result.Success(answer);
		}

		public EntitiesResponseWithCountAndPages<T> ApplyPaginationToEntities(List<T> entities, IPaginationFilter paginationFilter)
		{
			var pagesCount = (int)Math.Ceiling((double)entities.Count / paginationFilter.Limit);
			var currentPage = entities.Skip((paginationFilter.Page - 1) * paginationFilter.Limit).Take(paginationFilter.Limit).ToList();
			return new EntitiesResponseWithCountAndPages<T>(currentPage, entities.Count, pagesCount);
		}

		public List<T> DoSorting<TOption, TOptionType>(List<T> entities, TOption? sortOption, Dictionary<TOption?, Func<T, TOptionType>> orderByExpressions, bool isAscendingSorting)
		{
			if (sortOption == null) return entities;
			var orderedEntities = isAscendingSorting ? entities.OrderBy(orderByExpressions[sortOption]).ToList()
				: entities.OrderByDescending(orderByExpressions[sortOption]).ToList();
			return orderedEntities;
		}
	}
}
