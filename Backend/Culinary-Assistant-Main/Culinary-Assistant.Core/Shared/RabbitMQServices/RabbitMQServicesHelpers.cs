using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Shared.ProducerServices
{
	public static class RabbitMQServicesHelpers
	{
		public static async Task DoExchangeAndQueueDeclaringAndBindingAsync(IChannel channel, string exchangeName, string exchangeType, string queueName, string routingKey)
		{
			await channel.ExchangeDeclareAsync(exchangeName, exchangeType);
			await channel.QueueDeclareAsync(queueName);
			await channel.QueueBindAsync(queueName, exchangeName, routingKey);
		}
	}
}
