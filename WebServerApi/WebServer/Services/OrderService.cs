using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.DB;
using WebServer.Dto;
using WebServer.Interfaces;
using WebServer.Models;

namespace WebServer.Services
{
	public class OrderService : IOrderService
	{
		private readonly IMapper _mapper;
		private readonly DataBaseUserContext _dbContext;
		private readonly IConfigurationSection _secretKey;

		public OrderService(IMapper mapper, DataBaseUserContext dbContext, Microsoft.Extensions.Configuration.IConfiguration config)
		{
			_mapper = mapper;
			_dbContext = dbContext;
			_secretKey = config.GetSection("SecretKey");
		}

		public OrderDto AcceptOrder(OrderDto order, IHeaderDictionary headers)
		{
			Order newOrder = _mapper.Map<Order>(order);



			return null;
		}

		public OrderDto AddOrder(OrderDto order, IHeaderDictionary headers)
		{
			Order newOrder = _mapper.Map<Order>(order);

			return null;		}

		public IEnumerable<OrderDto> GetUserOrders(IHeaderDictionary headers)
		{
			throw new NotImplementedException();
		}
	}
}
