using Core.Base.Interfaces;
using Culinary_Assistant.Core.Base;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.ProducerServices;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using Culinary_Assistant_Main.Domain.Models.Interfaces;
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
using System.Threading;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.RabbitMQ.Consumers.Abstract
{
	public abstract class RatingMessagesConsumer<T, TRated>(IOptions<RabbitMQOptions> options, IServiceScopeFactory serviceScopeFactory, ILogger logger)
		: BaseConsumerService(options, serviceScopeFactory) where T: Rate<T, TRated>, new() where TRated: Core.Base.Entity<Guid>
	{
		private readonly ILogger _logger = logger;

		public override async Task OnReceived(object sender, BasicDeliverEventArgs e)
		{
			try
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var entitiesRepository = scope.ServiceProvider.GetRequiredService<IRepository<TRated>>();
				var ratesService = scope.ServiceProvider.GetRequiredService<IRateService<T, TRated>>();
				var messageString = Encoding.UTF8.GetString(e.Body.ToArray());
				var isGuid = Guid.TryParse(messageString, out Guid entityId);
				if (!isGuid)
				{
					_logger.Error("Ошибка обработки рейтинга сущности. Передан не Guid!");
					return;
				}
				var entity = await entitiesRepository.GetBySelectorAsync(e => e.Id == entityId);
				if (entity == null || entity is not IRateable rateable)
				{
					_logger.Error("Ошибка обработки рейтинга сущности {@id}. Несуществующая или неоцениваемая сущность!", entityId);
					return;
				}
				var rates = await ratesService.GetAllRatesForEntityAsync(entityId);
				var rating = CalculateRating(rates);
				rateable.SetRating(rating);
				await entitiesRepository.SaveChangesAsync();
				_logger.Information("Обновлен рейтинг для сущности {@id}", entityId);
			}
			finally
			{
				await _channel.BasicAckAsync(e.DeliveryTag, false);
			}
		}

		private static double CalculateRating(List<T> rates)
		{
			if (rates.Count == 0) return 0.0;
			var sumRate = rates.Sum(r => r.Rating);
			return (double)sumRate / rates.Count;
		}

		protected async Task ConfigureConsumerAsync(string consumeQueue, CancellationToken cancellationToken)
		{
			var consumer = new AsyncEventingBasicConsumer(_channel);
			consumer.ReceivedAsync += OnReceived;
			await _channel.BasicConsumeAsync(consumeQueue, false, consumer, cancellationToken);
		}
	}
}
