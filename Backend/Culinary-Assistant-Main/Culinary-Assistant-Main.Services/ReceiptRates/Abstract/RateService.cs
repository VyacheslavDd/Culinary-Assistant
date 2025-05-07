using Core.Base.Interfaces;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.ReceiptRate;
using Culinary_Assistant_Main.Domain.Fabrics;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.RabbitMQ.ReceiptRates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.ReceiptRates.Abstract
{
    public abstract class RateService<T, TRated>(IRatesRepository<T, TRated> ratesRepository, IUsersRepository usersRepository,
        IRepository<TRated> entitiesRepository) : IRateService<T, TRated> where T: Rate<T, TRated>, new() where TRated: Core.Base.Entity<Guid>
    {
        private readonly IRatesRepository<T, TRated> _ratesRepository = ratesRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        protected readonly IRepository<TRated> _entitiesRepository = entitiesRepository;

        protected async Task<Result> AddOrUpdateAsync(RateModelDTO rateModelDTO, Func<TRated, Task> onFirstRate, Func<TRated, Task> onRepeatedRate)
        {
			var user = await _usersRepository.GetBySelectorAsync(u => u.Id == rateModelDTO.UserId);
			if (user == null) return Result.Failure("Несуществующий пользователь");
			var entity = await _entitiesRepository.GetBySelectorAsync(r => r.Id == rateModelDTO.EntityId);
			if (entity == null) return Result.Failure("Несуществующая сущность");
			var rate = await GetAsync(rateModelDTO.UserId, rateModelDTO.EntityId);
			if (rate == null)
			{
				var rateRes = RateFactory.Create<T, TRated>(rateModelDTO);
				if (rateRes.IsFailure) return Result.Failure(rateRes.Error);
				await _ratesRepository.AddAsync(rateRes.Value);
				await onFirstRate(entity);
				return Result.Success();
			}
			var setRateRes = rate.SetRate(rateModelDTO.Rate);
			if (setRateRes.IsFailure) return Result.Failure(setRateRes.Error);
			await _ratesRepository.SaveChangesAsync();
			await onRepeatedRate(entity);
			return Result.Success();
		}

		public abstract Task<Result> AddOrUpdateAsync(RateModelDTO rateModelDTO);

        public async Task<List<T>> GetAllRatesForEntityAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _ratesRepository.GetAllRatesForEntity(id).ToListAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        {
            return await _ratesRepository.GetAsync(userId, id, cancellationToken);
        }
    }
}
