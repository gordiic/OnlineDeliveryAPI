using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WebServer.DB;
using WebServer.Dto;
using WebServer.Interfaces;
using WebServer.Mapping;
using WebServer.Models;

namespace WebServer.Services
{
	public class OrderService : IOrderService
	{
		private readonly IMapper _mapper;
		private readonly MyMapper _mymapper;

		private readonly DataBaseUserContext _dbContext;
		private readonly IConfigurationSection _secretKey;

		public OrderService(IMapper mapper, DataBaseUserContext dbContext, Microsoft.Extensions.Configuration.IConfiguration config)
		{
			_mapper = mapper;
			_dbContext = dbContext;
			_secretKey = config.GetSection("SecretKey");
			_mymapper = new MyMapper();
		}

		public void DeliveryThread(object order)
		{
			Order o = order as Order;
			Thread.Sleep(o.DeliverTime*1000);
			o.Done = true;
			_dbContext.orders.Update(o);
			_dbContext.SaveChanges(); 
		}

		public OrderDto AcceptOrder(int id, string token)
		{
			var result = _dbContext.orders.Where(a => a.Id == id).ToList();
			Order order = result[0];
			//provjeriti proivode da li se diraju
			order.DelivererId = id;
			order.AcceptanceTime = DateTime.Now.ToString();
			order.Accepted = true;
			Thread newThread = new Thread(DeliveryThread);
			newThread.Start(order);
			return _mapper.Map<OrderDto>(order);
		}

		public OrderDto AddOrder(OrderDto order, IHeaderDictionary headers)
		{
			Order newOrder = _mapper.Map<Order>(order);
			var handler = new JwtSecurityTokenHandler();
			string authHeader = headers["Authorization"];
			authHeader = authHeader.Replace("Bearer ", "");
			var jsonToken = handler.ReadToken(authHeader);
			var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
			var stringId = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
			int id = int.Parse(stringId);
			newOrder.UserId = id;
			List<OrderProduct> products = new List<OrderProduct>();
			//int id, string name, string ingredients, double price, double amount
			foreach (OrderProduct op in newOrder.Products)
			{
				products.Add(new OrderProduct(0,op.Name,op.Ingredients,op.Price,op.Amount));
			}
			newOrder.Products = new List<OrderProduct>();
			EntityEntry ee =  _dbContext.orders.Add(newOrder);
			_dbContext.SaveChanges();//baca error

			foreach (OrderProduct op in products)
			{
				Order o =_mapper.Map<Order>(ee.Entity);
				op.OrderId = o.Id;
				_dbContext.orderProducts.Add(op);
			}
			_dbContext.SaveChanges();//baca error
			List<User> users = _dbContext.users.Include(a => a.Orders).ToList();
			List<Order> orders = _dbContext.orders.Include(a => a.Products).ToList();
			return _mapper.Map<OrderDto>(ee.Entity);
		}

		public IEnumerable<OrderDto> GetNewOrders(IHeaderDictionary headers)
		{
			List<Order> orders = _dbContext.orders.Where(o => o.Accepted == false).Include(o => o.Products).ToList();


			List<OrderDto> ret = _mymapper.MapOrderToOrderDto(orders);

			return ret;
		}

		public IEnumerable<OrderDto> GetUserOrders(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(token);
			var tokenS = handler.ReadToken(token) as JwtSecurityToken;
			var stringId = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
			int id = int.Parse(stringId);

			List<Order> orders = _dbContext.orders.Where(o => o.UserId == id).Include(o=>o.Products).ToList();
			foreach(Order o in orders)
			{
				foreach (OrderProduct p in o.Products)
				{
					p.Order = null;
					p.OrderId = 0;
				}
				//o.Products = new List<OrderProduct>();
			}


			List<OrderDto> ret = _mymapper.MapOrderToOrderDto(orders);
		
			return ret;
		}
	}
}
