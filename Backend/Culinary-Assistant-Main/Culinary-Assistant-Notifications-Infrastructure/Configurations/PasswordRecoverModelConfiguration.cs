using Culinary_Assistant_Notifications_Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Notifications_Infrastructure.Configurations
{
	public class PasswordRecoverModelConfiguration : IEntityTypeConfiguration<PasswordRecover>
	{
		public void Configure(EntityTypeBuilder<PasswordRecover> builder)
		{
			builder.OwnsOne(pr => pr.UserEmail);
		}
	}
}
