using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Dto;
using WebServer.Models;

namespace WebServer.DB.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasKey(x => x.Id); //Podesavam primarni kljuc tabele
			builder.Property(x => x.Id).ValueGeneratedOnAdd();
			builder.HasIndex(x => x.UserName).IsUnique();//kazem da je maks duzina 30 karaktera
			builder.HasIndex(x => x.Email).IsUnique();
			builder.Property(x => x.Password).HasMaxLength(450);
			builder.Property(x => x.Name).HasMaxLength(30);
			builder.Property(x => x.LastName).HasMaxLength(30);
			builder.Property(x => x.BirthDate);
			builder.Property(x => x.Address).HasMaxLength(30);
			builder.Property(x => x.UserType).HasMaxLength(30);
			builder.HasMany(x => x.Orders)
				   .WithOne(x => x.User);
		}
	}
}
