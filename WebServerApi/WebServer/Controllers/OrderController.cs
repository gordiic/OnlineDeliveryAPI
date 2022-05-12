using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Dto;
using WebServer.Interfaces;
using WebServer.Models;

namespace WebServer.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}
		[HttpPost]
		[Route("addorder")]
		public IActionResult AddOrder(OrderDto order)
		{
			return Ok(_orderService.AddOrder(order, Request.Headers));
		}

		[HttpGet]
		[Route("getuserorders")]
		public IActionResult GetUserOrder()
		{
			return Ok(_orderService.GetUserOrders(Request.Headers));
		}

		[HttpPost]
		[Route("acceptorder")]
		public IActionResult AcceptOrder(OrderDto order)
		{
			return Ok(_orderService.AcceptOrder(order, Request.Headers));
		}
	}
}
