using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.RabbitMQ.ReceiptRates
{
	public class ReceiptRatingMessageProducerService(IOptions<RabbitMQOptions> options, ILogger logger) : IReceiptRatingMessageProducerService
	{
		private readonly RabbitMQOptions _rabbitMQOptions = options.Value;
		private readonly ILogger _logger = logger;

		public async Task SendUpdateReceiptRatingMessageAsync(Guid receiptId)
		{
			var factory = new ConnectionFactory() { HostName = _rabbitMQOptions.HostName };
			using var connection = await factory.CreateConnectionAsync();
			using var channel = await connection.CreateChannelAsync();
			await channel.ExchangeDeclareAsync(RabbitMQConstants.RatingExchangeName, ExchangeType.Topic);
			var message = Encoding.UTF8.GetBytes(receiptId.ToString());
			await channel.BasicPublishAsync(RabbitMQConstants.RatingExchangeName, "rating.update.receipt", message);
			_logger.Information("Рецепт {@id}: опубликовано сообщение на обновление рейтинга", receiptId);
		}
	}
}
