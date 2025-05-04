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
	public class ReceiptRateConfiguration : IEntityTypeConfiguration<ReceiptRate>
	{
		public void Configure(EntityTypeBuilder<ReceiptRate> builder)
		{
			builder.HasOne(rr => rr.User)
				.WithMany(u => u.ReceiptRates)
				.HasForeignKey(rr => rr.UserId)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}
}
