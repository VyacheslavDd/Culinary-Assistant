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
	public class ReceiptCollectionLikeConfiguration : IEntityTypeConfiguration<ReceiptCollectionLike>
	{
		public void Configure(EntityTypeBuilder<ReceiptCollectionLike> builder)
		{
			builder.Property(rcl => rcl.LikedEntityId).HasColumnName("ReceiptCollectionId");
			builder.HasOne(rcl => rcl.Entity).WithMany(rc => rc.Likes).HasForeignKey(rcl => rcl.LikedEntityId);
		}
	}
}
