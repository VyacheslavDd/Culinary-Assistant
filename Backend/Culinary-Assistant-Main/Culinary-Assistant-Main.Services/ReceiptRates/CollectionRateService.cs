using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.ReceiptRate;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.RabbitMQ.ReceiptRates;
using Culinary_Assistant_Main.Services.ReceiptRates.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.ReceiptRates
{
	public class CollectionRateService(IRatingMessageProducerService<ReceiptCollection> ratingMessageProducerService, IRatesRepository<ReceiptCollectionRate, ReceiptCollection> ratesRepository,
		IReceiptCollectionsRepository receiptCollectionsRepository, IUsersRepository usersRepository)
		: RateService<ReceiptCollectionRate, ReceiptCollection>(ratesRepository, usersRepository, receiptCollectionsRepository)
	{
		private readonly IRatingMessageProducerService<ReceiptCollection> _ratingMessageProducerService = ratingMessageProducerService;

		public override async Task<Result> AddOrUpdateAsync(RateModelDTO rateModelDTO)
		{
			return await AddOrUpdateAsync(rateModelDTO, OnFirstRate, OnRepeatedRate);
		}

		private async Task OnFirstRate(ReceiptCollection receiptCollection)
		{
			receiptCollection.AddPopularity();
			await _entitiesRepository.SaveChangesAsync();
			await _ratingMessageProducerService.SendUpdateRatingMessageAsync(receiptCollection.Id);
		}

		private async Task OnRepeatedRate(ReceiptCollection receiptCollection) => await _ratingMessageProducerService.SendUpdateRatingMessageAsync(receiptCollection.Id);
	}
}
