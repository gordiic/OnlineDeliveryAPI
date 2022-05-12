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

		IEnumerable<OrderDto> GetUserOrders(IHeaderDictionary headers);

		OrderDto AcceptOrder(OrderDto order, IHeaderDictionary headers);
	}
}
