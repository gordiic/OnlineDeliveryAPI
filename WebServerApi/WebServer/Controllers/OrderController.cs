using Microsoft.AspNetCore.Authorization;
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
		[Authorize(Roles = "ordinaryUser")]
		public IActionResult AddOrder(OrderDto order)
		{
			return Ok(_orderService.AddOrder(order, Request.Headers));
		}

		[HttpGet]
		[Route("getuserorders")]
		[Authorize(Roles = "deliverer,ordinaryUser,administrator")]
		public IActionResult GetUserOrder(string token)
		{
			List<OrderDto> o = _orderService.GetUserOrders(token).ToList();
			return Ok(o);
		}

		[HttpPost]
		[Route("getneworders")]
		[Authorize(Roles = "deliverer")]
		public IActionResult GetNewOrders()
		{
			return Ok(_orderService.GetNewOrders(Request.Headers));
		}

		[HttpGet]
		[Route("acceptorder")]
		[Authorize(Roles = "deliverer")]
		public IActionResult AcceptOrder(int id, string token)
		{
			return Ok(_orderService.AcceptOrder(id,token));
		}

		[HttpGet]
		[Route("getcurrentorder")]
		public IActionResult GetCurrentOrder(string token)
		{
			
			return Ok(_orderService.GetCurrentOrder(token));
		}

	}
}
