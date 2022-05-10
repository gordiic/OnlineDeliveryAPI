using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models
{
	[Table("OrderProducts", Schema = "dbo")]
	public class OrderProduct
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Ingredients { get; set; }
		public double Price { get; set; }
		public double Amount { get; set; }
		public Order Order { get; set; }
		[ForeignKey("Order")]
		public int OrderId { get; set; }

		public OrderProduct()
		{

		}
		public OrderProduct(int id, string name, string ingredients, double price, double amount)
		{
			this.Id = id;
			this.Name = name;
			this.Ingredients = ingredients;
			this.Price = price;
			this.Amount = amount;
		}
	}
}
