using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Services.RabbitMQ.ReceiptRates.Abstract;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.RabbitMQ.ReceiptRates
{
	public class CollectionRatingMessageProducerService(IOptions<RabbitMQOptions> options, ILogger logger) : RatingMessageProducerService<ReceiptCollection>(options, logger)
	{
		public override async Task SendUpdateRatingMessageAsync(Guid collectionId)
		{
			await SendUpdateReceiptRatingMessageAsync(collectionId, RabbitMQConstants.UpdateCollectionRatingRoutingKey);
		}
	}
}
