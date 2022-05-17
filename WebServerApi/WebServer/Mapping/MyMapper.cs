using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Dto;
using WebServer.Models;

namespace WebServer.Mapping
{
	public class MyMapper
	{
		public List<OrderDto> MapOrderToOrderDto(List<Order> orders)
		{
			List<OrderDto> ret = new List<OrderDto>();

			foreach (Order o in orders)
			{
				OrderDto od = new OrderDto();
				od.Products = new List<OrderProductDto>();
				od.AcceptanceTime = o.AcceptanceTime;
				od.Accepted = o.Accepted;
				od.Address = o.Address;
				od.Comment = o.Comment;
				od.DelivererId = o.DelivererId;
				od.DeliverTime = o.DeliverTime;
				od.Done = o.Done;
				od.Id = o.Id;
				od.Price = o.Price;
				od.UserId = o.UserId;

				foreach (OrderProduct op in o.Products)
				{
					OrderProductDto opd = new OrderProductDto();
					opd.Id = op.Id;
					opd.Ingredients = op.Ingredients;
					opd.Name = op.Name;
					opd.Order = new OrderDto();
					opd.OrderId = 0;
					opd.Price = op.Price;
					opd.Amount = op.Amount;

					od.Products.Add(opd);
				}
				ret.Add(od);
			}


			return ret;
		}
	}
}
