using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Dto;

namespace WebServer.Interfaces
{
	public interface IOrderService
	{
		OrderDto AddOrder(OrderDto order, IHeaderDictionary headers);

		IEnumerable<OrderDto> GetUserOrders(string token);

		OrderDto AcceptOrder(int id, string token);
		IEnumerable<OrderDto> GetNewOrders(IHeaderDictionary headers);

		OrderDto GetCurrentOrder(string token);
	}
}
