using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Dto;
using WebServer.Models;

namespace WebServer.DB
{
	public class DataBaseUserContext : DbContext
	{
        public DbSet<User> users { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<OrderProduct> orderProducts { get; set; }
        public DbSet<Order> orders { get; set; }

		public DataBaseUserContext(DbContextOptions options) : base(options)
		{

		}
		

		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		protected override void OnModelCreating(ModelBuilder modelBuilder)
       
        {
			//string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OnlineDostavaDataBase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
			////connectionString = DataBaseUserContext
			//optionsBuilder.UseSqlServer(connectionString);

			base.OnModelCreating(modelBuilder);
			//Kazemo mu da pronadje sve konfiguracije u Assembliju i da ih primeni nad bazom
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataBaseUserContext).Assembly);
		}
    }
}
