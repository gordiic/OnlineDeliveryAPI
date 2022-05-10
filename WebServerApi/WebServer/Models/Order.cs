using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models
{
	[Table("Orders", Schema = "dbo")]
	public class Order
	{
		public int Id { get; set; }
		public ICollection<OrderProduct> Products { get; set; }
		public string Address { get; set; }
		public string Comment { get; set; }
		public double Price { get; set; }
		public int DeliverTime { get; set; }
		public bool Done { get; set; }
		public bool Accepted { get; set; }
		public string AcceptanceTime { get; set; }
		public int DelivererId { get; set; }
		[ForeignKey("User")]
		public int UserId { get; set; }
		public User User { get; set; }

		public Order(int id, List<OrderProduct> products, string address, string comment, double price, int deliverTime, bool done,bool accepted, string acceptanceTime, int delivererId, int userId)
		{
			this.Id = id;
			this.Products = products;
			this.Address = address;
			this.Comment = comment;
			this.Price = price;
			this.DeliverTime = deliverTime;
			this.Done = done;
			this.Accepted = accepted;
			this.AcceptanceTime = acceptanceTime;
			this.DelivererId = delivererId;
			this.UserId = userId;
		}

		public Order()
		{

		}
	}
}
