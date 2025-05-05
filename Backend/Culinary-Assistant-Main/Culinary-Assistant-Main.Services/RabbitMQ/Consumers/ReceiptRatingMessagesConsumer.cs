using Culinary_Assistant.Core.Base;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.ProducerServices;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using Culinary_Assistant_Main.Services.RabbitMQ.Consumers.Abstract;
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
    public class ReceiptRatingMessagesConsumer(IOptions<RabbitMQOptions> options, IServiceScopeFactory serviceScopeFactory, ILogger logger)
		: RatingMessagesConsumer<ReceiptRate, Receipt>(options, serviceScopeFactory, logger)
	{
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			await base.ExecuteAsync(cancellationToken);
			await RabbitMQServicesHelpers.DoExchangeAndQueueDeclaringAndBindingAsync(_channel, RabbitMQConstants.RatingExchangeName, ExchangeType.Topic, RabbitMQConstants.UpdateReceiptRatingQueue,
				RabbitMQConstants.UpdateReceiptRatingRoutingKey);
			await ConfigureConsumerAsync(RabbitMQConstants.UpdateReceiptRatingQueue, cancellationToken);
		}
	}
}
