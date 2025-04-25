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
	public class ReceiptModelConfiguration : IEntityTypeConfiguration<Receipt>
	{
		public void Configure(EntityTypeBuilder<Receipt> builder)
		{
			builder.OwnsOne(r => r.Title);
			builder.OwnsOne(r => r.Description);
			builder.OwnsOne(r => r.Nutrients);
		}
	}
}
