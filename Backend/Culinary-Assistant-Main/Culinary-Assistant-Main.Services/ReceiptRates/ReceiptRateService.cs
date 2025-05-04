using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.ReceiptRate;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Services.RabbitMQ.ReceiptRates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.ReceiptRates
{
	public class ReceiptRateService(IReceiptRatingMessageProducerService receiptRatingMessageProducerService, IReceiptRatesRepository receiptRatesRepository, IUsersRepository usersRepository,
		IReceiptsRepository receiptsRepository) : IReceiptRateService
	{
		private readonly IReceiptRatingMessageProducerService _receiptRatingMessageProducerService = receiptRatingMessageProducerService;
		private readonly IReceiptRatesRepository _receiptRatesRepository = receiptRatesRepository;
		private readonly IUsersRepository _usersRepository = usersRepository;
		private readonly IReceiptsRepository _receiptsRepository = receiptsRepository;

		public async Task<Result> AddOrUpdateAsync(ReceiptRateModelDTO receiptRateModelDTO)
		{
			var user = await _usersRepository.GetBySelectorAsync(u => u.Id == receiptRateModelDTO.UserId);
			if (user == null) return Result.Failure("Несуществующий пользователь");
			var receipt = await _receiptsRepository.GetBySelectorAsync(r => r.Id == receiptRateModelDTO.ReceiptId);
			if (receipt == null) return Result.Failure("Несуществующий рецепт");
			var rate = await GetAsync(receiptRateModelDTO.UserId, receiptRateModelDTO.ReceiptId);
			if (rate == null)
			{
				var rateRes = ReceiptRate.Create(receiptRateModelDTO);
				if (rateRes.IsFailure) return Result.Failure(rateRes.Error);
				await _receiptRatesRepository.AddAsync(rateRes.Value);
				await _receiptRatingMessageProducerService.SendUpdateReceiptRatingMessageAsync(receipt.Id);
				return Result.Success();
			}
			var setRateRes = rate.SetRate(receiptRateModelDTO.Rate);
			if (setRateRes.IsFailure) return Result.Failure(setRateRes.Error);
			await _receiptRatesRepository.SaveChangesAsync();
			await _receiptRatingMessageProducerService.SendUpdateReceiptRatingMessageAsync(receipt.Id);
			return Result.Success();
		}

		public async Task<List<ReceiptRate>> GetAllRatesForReceiptAsync(Guid receiptId, CancellationToken cancellationToken = default)
		{
			return await _receiptRatesRepository.GetAllRatesForReceipt(receiptId).ToListAsync(cancellationToken);
		}

		public async Task<ReceiptRate?> GetAsync(Guid userId, Guid receiptId, CancellationToken cancellationToken = default)
		{
			return await _receiptRatesRepository.GetAsync(userId, receiptId, cancellationToken);
		}
	}
}
