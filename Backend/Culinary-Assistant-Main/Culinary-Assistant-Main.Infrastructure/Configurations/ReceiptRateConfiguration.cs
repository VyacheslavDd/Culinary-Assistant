using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Models.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Configurations
{
    public class ReceiptRateConfiguration : IEntityTypeConfiguration<ReceiptRate>
	{
		public void Configure(EntityTypeBuilder<ReceiptRate> builder)
		{
			builder.HasOne(rr => rr.User)
				.WithMany(u => u.ReceiptRates)
				.HasForeignKey(rr => rr.UserId)
				.OnDelete(DeleteBehavior.SetNull);
			builder.HasOne(rr => rr.Entity)
				.WithMany(e => e.Rates)
				.HasForeignKey(rr => rr.EntityId);
			builder.Property(rr => rr.EntityId).HasColumnName("ReceiptId");
		}
	}
}
