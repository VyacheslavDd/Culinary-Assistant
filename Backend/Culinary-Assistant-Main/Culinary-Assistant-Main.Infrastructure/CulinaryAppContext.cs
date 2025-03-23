using Culinary_Assistant.Core.Options;
using Culinary_Assistant_Main.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure
{
	public class CulinaryAppContext(DbContextOptions<CulinaryAppContext> dbContextOptions, IOptions<ConnectionOptions> connectionOptions, bool isTesting=false) 
		: DbContext(dbContextOptions)
	{
		private readonly ConnectionOptions _connectionOptions = connectionOptions.Value;

		public virtual DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("MainAppSchema");
			base.OnModelCreating(modelBuilder);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!isTesting)
				optionsBuilder.UseNpgsql(_connectionOptions.ConnectionString, options => options.MigrationsAssembly(Assembly.GetExecutingAssembly()));
			base.OnConfiguring(optionsBuilder);
		}
	}
}
