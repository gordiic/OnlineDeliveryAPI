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
	public class OrderProductConfiguration : IEntityTypeConfiguration<OrderProduct>
	{
		public void Configure(EntityTypeBuilder<OrderProduct> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Id).ValueGeneratedOnAdd();
			builder.HasOne(x => x.Order) 
				   .WithMany(x => x.Products) 
				   .HasForeignKey(x => x.OrderId) 
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
