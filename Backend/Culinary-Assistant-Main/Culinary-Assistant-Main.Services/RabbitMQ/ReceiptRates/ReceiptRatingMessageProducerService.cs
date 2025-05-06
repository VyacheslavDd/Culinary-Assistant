using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Services.RabbitMQ.ReceiptRates.Abstract;
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
	public class ReceiptRatingMessageProducerService(IOptions<RabbitMQOptions> options, ILogger logger) : RatingMessageProducerService<Receipt>(options, logger)
	{
		public override async Task SendUpdateRatingMessageAsync(Guid receiptId)
		{
			await SendUpdateReceiptRatingMessageAsync(receiptId, RabbitMQConstants.UpdateReceiptRatingRoutingKey);
		}
	}
}
