using Culinary_Assistant_Notifications_Services.PasswordRecoverService;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Notifications_Services.Jobs
{
	public class RemoveOutdatedPasswordRecoverRequestsJob(IPasswordRecoversService passwordRecoversService, ILogger logger) : IJob
	{
		private readonly IPasswordRecoversService _passwordRecoversService = passwordRecoversService;
		private readonly ILogger _logger = logger;

		public async Task Execute(IJobExecutionContext context)
		{
			var removedRowsCount = await _passwordRecoversService.DeleteOutdatedRecoversAsync();
			_logger.Information("Удалено запросов на восстановление пароля: {@count}", removedRowsCount);
		}
	}
}
