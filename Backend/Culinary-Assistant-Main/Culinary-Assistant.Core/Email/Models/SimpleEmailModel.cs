using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Email.Models
{
	public record SimpleEmailModel(string Subject, string Body, string Sender, string Receiver) : BaseEmailModel(Subject, Body, Sender, Receiver);
}
