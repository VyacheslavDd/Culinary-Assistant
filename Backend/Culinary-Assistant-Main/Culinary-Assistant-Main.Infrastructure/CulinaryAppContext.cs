using Culinary_Assistant.Core.Options;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using Culinary_Assistant_Main.Infrastructure.Configurations;
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
		public virtual DbSet<Receipt> Receipts { get; set; }
		public virtual DbSet<ReceiptCollection> ReceiptCollections { get; set; }
		public virtual DbSet<ReceiptLike> ReceiptLikes { get; set; }
		public virtual DbSet<ReceiptCollectionLike> ReceiptCollectionLikes { get; set; }
		public virtual DbSet<ReceiptRate> ReceiptRates { get; set; }
		public virtual DbSet<ReceiptCollectionRate> ReceiptCollectionRates { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("MainAppSchema");
			new UserModelConfiguration().Configure(modelBuilder.Entity<User>());
			new ReceiptModelConfiguration().Configure(modelBuilder.Entity<Receipt>());
			new ReceiptCollectionModelConfiguration().Configure(modelBuilder.Entity<ReceiptCollection>());
			new ReceiptCollectionLikeConfiguration().Configure(modelBuilder.Entity<ReceiptCollectionLike>());
			new ReceiptLikeConfiguration().Configure(modelBuilder.Entity<ReceiptLike>());
			new ReceiptRateConfiguration().Configure(modelBuilder.Entity<ReceiptRate>());
			new ReceiptCollectionRateConfiguration().Configure(modelBuilder.Entity<ReceiptCollectionRate>());
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
