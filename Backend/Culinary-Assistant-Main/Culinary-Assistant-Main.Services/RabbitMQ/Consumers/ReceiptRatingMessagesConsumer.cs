using Culinary_Assistant.Core.Base;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.ProducerServices;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using Culinary_Assistant_Main.Services.ReceiptRates;
using Culinary_Assistant_Main.Services.Receipts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.RabbitMQ.Consumers
{
    public class ReceiptRatingMessagesConsumer(IOptions<RabbitMQOptions> options, IServiceScopeFactory serviceScopeFactory, ILogger logger) : BaseConsumerService(options, serviceScopeFactory)
	{
		private readonly ILogger _logger = logger;

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			await base.ExecuteAsync(cancellationToken);
			await RabbitMQServicesHelpers.DoExchangeAndQueueDeclaringAndBindingAsync(_channel, RabbitMQConstants.RatingExchangeName, ExchangeType.Topic, RabbitMQConstants.UpdateRatingQueue,
				RabbitMQConstants.UpdateRatingRoutingKey);
			var consumer = new AsyncEventingBasicConsumer(_channel);
			consumer.ReceivedAsync += OnReceived;
			await _channel.BasicConsumeAsync(RabbitMQConstants.UpdateRatingQueue, false, consumer, cancellationToken);

		}

		public override async Task OnReceived(object sender, BasicDeliverEventArgs e)
		{
			try
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var receiptsService = scope.ServiceProvider.GetRequiredService<IReceiptsService>();
				var receiptRatesService = scope.ServiceProvider.GetRequiredService<IRateService<ReceiptRate, Receipt>>();
				var messageString = Encoding.UTF8.GetString(e.Body.ToArray());
				var isGuid = Guid.TryParse(messageString, out Guid receiptId);
				if (!isGuid)
				{
					_logger.Error("Ошибка обработки рейтинга рецепта. Передан не Guid!");
					return;
				}
				var doesReceiptExist = (await receiptsService.GetByGuidAsync(receiptId)) != null;
				if (!doesReceiptExist)
				{
					_logger.Error("Ошибка обработки рейтинга рецепта {@id}. Несуществующий рецепт!", receiptId);
					return;
				}
				var receiptRates = await receiptRatesService.GetAllRatesForEntityAsync(receiptId);
				var rating = CalculateRating(receiptRates);
				var setRatingRes = await receiptsService.SetRatingAsync(receiptId, rating);
				if (setRatingRes.IsFailure)
				{
					_logger.Error("Ошибка сохранения рейтинга рецепта {@id}. {@error}", receiptId, setRatingRes.Error);
					return;
				}
				_logger.Information("Обновлен рейтинг для рецепта {@id}", receiptId);
			}
			finally
			{
				await _channel.BasicAckAsync(e.DeliveryTag, false);
			}
		}

		private static double CalculateRating(List<ReceiptRate> rates)
		{
			if (rates.Count == 0) return 0.0;
			var sumRate = rates.Sum(r => r.Rating);
			return (double)sumRate / rates.Count;
		}
	}
}
