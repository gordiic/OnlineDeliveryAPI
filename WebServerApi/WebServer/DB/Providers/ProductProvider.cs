using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.DB.Providers
{
	public class ProductProvider
	{
		private readonly DataBaseUserContext _dbContext;
		public ProductProvider(DataBaseUserContext dbContext)
		{
			_dbContext = dbContext;
		}

		public List<Product> GetProducts()
		{
			List<Product> products = _dbContext.products.ToList();
			return products;
		}

		public Product AddProduct(Product newProduct)
		{
			EntityEntry ee = _dbContext.products.Add(newProduct);
			_dbContext.SaveChanges();
			return (Product)ee.Entity;

		}

	}
}
