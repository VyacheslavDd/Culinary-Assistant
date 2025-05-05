using Culinary_Assistant_Main.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Configurations
{
	internal class ReceiptCollectionRateConfiguration : IEntityTypeConfiguration<ReceiptCollectionRate>
	{
		public void Configure(EntityTypeBuilder<ReceiptCollectionRate> builder)
		{
			builder
				.HasOne(rcr => rcr.User)
				.WithMany(u => u.CollectionRates)
				.HasForeignKey(rcr => rcr.UserId)
				.OnDelete(DeleteBehavior.SetNull);
			builder
				.HasOne(rcr => rcr.Entity)
				.WithMany(rc => rc.Rates)
				.HasForeignKey(rcr => rcr.EntityId);
			builder.Property(rcr => rcr.EntityId).HasColumnName("ReceiptCollectionId");
		}
	}
}
