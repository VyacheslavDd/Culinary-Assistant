using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant_Main.Domain.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.RabbitMQ.ReceiptRates.Abstract
{
	public abstract class RatingMessageProducerService<T>(IOptions<RabbitMQOptions> options, ILogger logger) : IRatingMessageProducerService<T>
	{
		private readonly RabbitMQOptions _rabbitMQOptions = options.Value;
		private readonly ILogger _logger = logger;

		public abstract Task SendUpdateRatingMessageAsync(Guid entityId);

		protected async Task SendUpdateReceiptRatingMessageAsync(Guid entityId, string routingKey)
		{
			var factory = new ConnectionFactory() { HostName = _rabbitMQOptions.HostName };
			using var connection = await factory.CreateConnectionAsync();
			using var channel = await connection.CreateChannelAsync();
			await channel.ExchangeDeclareAsync(RabbitMQConstants.RatingExchangeName, ExchangeType.Topic);
			var message = Encoding.UTF8.GetBytes(entityId.ToString());
			await channel.BasicPublishAsync(RabbitMQConstants.RatingExchangeName, routingKey, message);
			_logger.Information("Опубликовано сообщение на обновление рейтинга сущности {@entityName} ({@id})", typeof(T).Name, entityId);
		}
	}
}
