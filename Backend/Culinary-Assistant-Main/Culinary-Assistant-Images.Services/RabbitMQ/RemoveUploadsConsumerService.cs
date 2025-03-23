﻿using Core.Minio.Service;
using Culinary_Assistant.Core.Base;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.ProducerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Culinary_Assistant_Images.Services.RabbitMQ
{
	public class RemoveUploadsConsumerService(IOptions<RabbitMQOptions> rabbitMQOptions, IServiceScopeFactory serviceScopeFactory)
		: BaseConsumerService(rabbitMQOptions, serviceScopeFactory)
	{
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			await base.ExecuteAsync(cancellationToken);
			await RabbitMQServicesHelpers.DoExchangeAndQueueDeclaringAndBindingAsync(_channel, RabbitMQConstants.ImagesExchangeName, ExchangeType.Topic,
				RabbitMQConstants.RemoveImagesQueue, RabbitMQConstants.RemoveUploadsRoutingKey);

			var consumer = new AsyncEventingBasicConsumer(_channel);
			consumer.ReceivedAsync += OnReceived;
			await _channel.BasicConsumeAsync(RabbitMQConstants.RemoveImagesQueue, true, consumer, cancellationToken);
		}

		public override async Task OnReceived(object sender, BasicDeliverEventArgs e)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
			var urlsData = JsonSerializer.Deserialize<List<string>>(Encoding.UTF8.GetString(e.Body.ToArray()));
			await fileService.DeleteFilesAsync(BucketConstants.ReceiptsImagesBucketName, urlsData);
		}
	}
}
