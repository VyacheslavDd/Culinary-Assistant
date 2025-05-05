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
	public class ReceiptRateService(IReceiptRatingMessageProducerService receiptRatingMessageProducerService, IRatesRepository<ReceiptRate, Receipt> ratesRepository,
		IReceiptsRepository receiptsRepository, IUsersRepository usersRepository) :
		RateService<ReceiptRate, Receipt>(ratesRepository, usersRepository, receiptsRepository)
	{
		private readonly IReceiptRatingMessageProducerService _receiptRatingMessageProducerService = receiptRatingMessageProducerService;

		public override async Task<Result> AddOrUpdateAsync(RateModelDTO rateModelDTO)
		{
			return await AddOrUpdateAsync(rateModelDTO, OnFirstRate, OnRepeatedRate);
		}

		private async Task OnFirstRate(Receipt receipt)
		{
			receipt.AddPopularity();
			await _entitiesRepository.SaveChangesAsync();
			await _receiptRatingMessageProducerService.SendUpdateReceiptRatingMessageAsync(receipt.Id);
		}

		private async Task OnRepeatedRate(Receipt receipt) => await _receiptRatingMessageProducerService.SendUpdateReceiptRatingMessageAsync(receipt.Id);
	}
}
