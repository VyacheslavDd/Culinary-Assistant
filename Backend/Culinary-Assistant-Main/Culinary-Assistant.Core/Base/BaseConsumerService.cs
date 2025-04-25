using Culinary_Assistant.Core.Base.Interfaces;
using Culinary_Assistant.Core.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Base
{
	public abstract class BaseConsumerService(IOptions<RabbitMQOptions> rabbitMqOptions, IServiceScopeFactory serviceScopeFactory)
		: BackgroundService, IBaseConsumerService
	{
		protected RabbitMQOptions _rabbitMQOptions = rabbitMqOptions.Value;
		protected IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
		protected IConnection _connection;
		protected IChannel _channel;

		public abstract Task OnReceived(object sender, BasicDeliverEventArgs e);

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			var factory = new ConnectionFactory() { HostName = _rabbitMQOptions.HostName };
			_connection = await factory.CreateConnectionAsync(cancellationToken);
			_channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			await _channel.CloseAsync(cancellationToken);
			await _connection.CloseAsync(cancellationToken);
			await base.StopAsync(cancellationToken);
		}
	}
}
