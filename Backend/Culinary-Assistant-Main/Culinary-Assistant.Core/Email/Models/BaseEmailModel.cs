using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Email.Models
{
	public abstract record BaseEmailModel(string Subject, string Body, string Sender, string Receiver);
}
