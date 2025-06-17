using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.Email.Models;
using Culinary_Assistant.Core.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Email
{
	public class EmailService(ILogger logger, IOptions<EmailOptions> options, IConfiguration configuration) : IEmailService
	{
		private readonly ILogger _logger = logger;
		private readonly EmailOptions _emailOptions = options.Value;
		private readonly string _smtpPassword = Environment.GetEnvironmentVariable(ConfigurationConstants.SMTPPassword) ?? string.Empty;
		private readonly string _frontendHost = configuration[ConfigurationConstants.FrontendVMHost]!;

		public async Task SendPasswordRecoveryEmailAsync(string to, Guid recoverId)
		{
			var message = new MimeMessage();
			var fromParsed = InternetAddress.TryParse(_emailOptions.Username, out var fromAddress);
			var toParsed = InternetAddress.TryParse(to, out var toAddress);
			if (!fromParsed || !toParsed)
			{
				_logger.Error("Некорректный адрес отправителя или получателя для отправки сообщения на восстановление пароля: {@from}, {@to}", _emailOptions.Username, to);
				return;
			}
			message.From.Add(fromAddress);
			message.To.Add(toAddress);
			message.Subject = "Восстановление пароля на сайте \"Помощник кулинара\"";
			var messageBody = new BodyBuilder()
			{
				HtmlBody = $"<p>Ваша ссылка на восстановление пароля: {_frontendHost}/password-recover/{recoverId}</p><p>Игнорируйте данное сообщение, если вы не запрашивали смену пароля.</p>"
			};
			message.Body = messageBody.ToMessageBody();
			await SendMessageAsync(message, to);
		}

		private async Task SendMessageAsync(MimeMessage message, string to)
		{
			try
			{
				var smtpClient = new SmtpClient();
				await smtpClient.ConnectAsync(_emailOptions.Host, _emailOptions.Port, MailKit.Security.SecureSocketOptions.StartTls);
				await smtpClient.AuthenticateAsync(_emailOptions.Username, _smtpPassword);
				await smtpClient.SendAsync(message);
				await smtpClient.DisconnectAsync(true);
				_logger.Information("Сообщение на восстановление пароля успешно отправлено адресату {@to}", to);
			}
			catch (Exception e)
			{
				_logger.Error("Ошибка при отправке сообщения на восстановление пароля {@from}, {@to}. {@error}", _emailOptions.Username, to, e.Message);
			}
		}
	}
}
