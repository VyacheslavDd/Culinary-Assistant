using Culinary_Assistant.Core.Const;
using Culinary_Assistant_Notifications_Services.Jobs;
using Quartz;

namespace Culinary_Assistant_Notifications.Startups
{
	public static class QuartzStartup
	{
		public static IServiceCollection AddQuartzAndJobs(this IServiceCollection services)
		{
			services.AddQuartz(c =>
			{
				var jobKey = new JobKey("PWRecover");
				c.AddJob<RemoveOutdatedPasswordRecoverRequestsJob>(c => c.WithIdentity(jobKey));

				c.AddTrigger(t =>
				{
					t.WithIdentity("PWRecoverTrigger")
					.ForJob(jobKey)
					.WithSimpleSchedule(s =>
					{
						s.WithIntervalInMinutes(MiscellaneousConstants.TimeMinutesDeletePasswordRecoversTriggerRepeat)
						.RepeatForever();
					});
				});
			});
			return services;
		}
	}
}
