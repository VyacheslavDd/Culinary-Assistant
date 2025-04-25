using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Base.Interfaces
{
	public interface IBaseConsumerService
	{
		Task OnReceived(object sender, BasicDeliverEventArgs e);
	}
}
