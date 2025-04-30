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
	public class ReceiptLikeConfiguration : IEntityTypeConfiguration<ReceiptLike>
	{
		public void Configure(EntityTypeBuilder<ReceiptLike> builder)
		{
			builder.Property(rl => rl.LikedEntityId).HasColumnName("ReceiptId");
			builder.HasOne(rl => rl.Entity).WithMany(r => r.Likes).HasForeignKey(rl => rl.LikedEntityId);
		}
	}
}
