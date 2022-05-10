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
	public class OrderConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.HasKey(x => x.Id); //Podesavam primarni kljuc tabele
			builder.Property(x => x.Id).ValueGeneratedOnAdd();
			builder.HasMany(x => x.Products)
				   .WithOne(x => x.Order);
			builder.HasOne(x => x.User) //Kazemo da Student ima jedan fakultet
				  .WithMany(x => x.Orders) // A jedan fakultet vise studenata
				  .HasForeignKey(x => x.UserId) // Strani kljuc  je facultyId
				  .OnDelete(DeleteBehavior.Cascade);// Ako se obrise fakultet kaskadno se
		}
	}
}
