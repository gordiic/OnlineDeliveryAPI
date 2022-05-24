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

		//public async Task DeliveryThread(object order)
		//{
		//	Order o = order as Order;
		//	//_dbContext.orders.Update(o);
		//	//_dbContext.SaveChanges();
		//	//Thread.Sleep(o.DeliverTime*1000);
		//	Thread.Sleep(3000);
		//	o.Done = true;
		//	_dbContext.orders.Update(o);
		//	await _dbContext.SaveChangesAsync(); 
		//}

		public OrderDto AcceptOrder(int id, string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(token);
			var tokenS = handler.ReadToken(token) as JwtSecurityToken;
			var stringId = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
			int did = int.Parse(stringId);
			var result = _dbContext.orders.Where(a => a.Id == id).Include(a=>a.Products).Include(a=>a.User).ToList();
			Order order = result[0];
			//provjeriti proivode da li se diraju
			order.DelivererId = did;
			order.AcceptanceTime = DateTime.Now.ToString();
			order.Accepted = true;
			_dbContext.Update(order);
			_dbContext.SaveChanges();
			//Thread newThread = new Thread(DeliveryThread);
			//newThread.IsBackground = true;
			//Task.Run(() => 
			//	T(order)
			//);

			OrderDto ret = new OrderDto(order.Id, new List<OrderProductDto>(), order.Address, order.Comment, order.Price,
				order.DeliverTime, order.Done, order.Accepted, order.AcceptanceTime, order.DelivererId, order.UserId);
			//newThread.Start(order);

			return ret;
		}
		//public  void  T(Order order)
		//{
			
		//	_dbContext.Update(order);
		//	_dbContext.SaveChanges();
		//	Thread.Sleep(3000);
				
		//	Task.Run(() =>
		//		D(order)
		//	);

		//}
		//public void D(Order order)
		//{			
		//		order.Done = true;
		//		_dbContext.orders.Update(order);
		//		_dbContext.SaveChanges();
			
		//}
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
			var userType = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;

			List<Order> orders;
			if ((Dto.UserType)Enum.Parse(typeof(Dto.UserType), userType) == Dto.UserType.deliverer)
			{
				orders = _dbContext.orders.Where(o => o.DelivererId == id).Include(o => o.Products).ToList();
			}
			else if((Dto.UserType)Enum.Parse(typeof(Dto.UserType), userType) == Dto.UserType.ordinaryUser)
			{
				orders = _dbContext.orders.Where(o => o.UserId == id).Include(o => o.Products).ToList();
			}
			else
			{
				orders = _dbContext.orders.Include(o => o.Products).ToList();
			}
			foreach(Order o in orders)
			{
				foreach (OrderProduct p in o.Products)
				{
					p.Order = null;
					p.OrderId = 0;
				}
			}

			List<OrderDto> ret = _mymapper.MapOrderToOrderDto(orders);
		
			return ret;
		}

		public OrderDto GetCurrentOrder(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(token);
			var tokenS = handler.ReadToken(token) as JwtSecurityToken;
			var stringId = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
			int id = int.Parse(stringId);

			List<Order> orders = _dbContext.orders.Where(o => (o.DelivererId == id && o.Done == false)).Include(o => o.Products).ToList();
			if (orders[0] == null)
				return null;
			return _mapper.Map<OrderDto>(orders[0]);
		}
	}
}
