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
	public class ReceiptCollectionModelConfiguration : IEntityTypeConfiguration<ReceiptCollection>
	{
		public void Configure(EntityTypeBuilder<ReceiptCollection> builder)
		{
			builder.OwnsOne(rc => rc.Title);
		}
	}
}
