using Culinary_Assistant.Core.Options;
using Culinary_Assistant_Notifications_Domain.Models;
using Culinary_Assistant_Notifications_Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Notifications_Infrastructure
{
	public class NotificationsContext(DbContextOptions<NotificationsContext> dbContextOptions, IOptions<ConnectionOptions> options, bool isTesting=false) : DbContext(dbContextOptions)
	{
		private readonly ConnectionOptions _connectionOptions = options.Value;
		private readonly bool _isTesting = isTesting;

		public virtual DbSet<PasswordRecover> PasswordRecovers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("NotificationsSchema");
			new PasswordRecoverModelConfiguration().Configure(modelBuilder.Entity<PasswordRecover>());
			base.OnModelCreating(modelBuilder);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!_isTesting)
				optionsBuilder.UseNpgsql(_connectionOptions.ConnectionString, o => o.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
			base.OnConfiguring(optionsBuilder);
		}
	}
}
