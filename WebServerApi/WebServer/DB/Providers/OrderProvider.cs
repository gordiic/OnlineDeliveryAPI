using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.DB.Providers
{
	public class OrderProvider
	{
		private readonly DataBaseUserContext _dbContext;
		public OrderProvider(DataBaseUserContext dbContext)
		{
			_dbContext = dbContext;
		}
		public Order UpdateOrder(Order order)
		{
			EntityEntry ee = _dbContext.Update(order);
			try
			{
				_dbContext.SaveChanges();
			}catch(Exception e)
			{
				return null;
			}
			return (Order)ee.Entity;
		}

		public Order AddOrder(Order newOrder)
		{
			EntityEntry ee = _dbContext.orders.Add(newOrder);
			_dbContext.SaveChanges();//baca error

			return (Order)ee.Entity;
		}

		public List<Order> GetAcceptedOrders()
		{
			return _dbContext.orders.Where(o => o.Accepted == false).Include(o => o.Products).ToList(); 
		}

		public List<Order> GetOrdersOfOrdinaryUser(int id)
		{
			return _dbContext.orders.Where(o => o.UserId == id).Include(o => o.Products).ToList();
		}

		public List<Order> GetOrdersOfDeliverer(int id)
		{
			return _dbContext.orders.Where(o => o.DelivererId == id).Include(o => o.Products).ToList();
		}

		public List<Order> GetAllOrders()
		{
			return _dbContext.orders.Include(o => o.Products).ToList();
		}
		
		public List<Order> GetCurrentOrder(int id)
		{
			return _dbContext.orders.Where(o => (o.DelivererId == id && o.Done == false)).Include(o => o.Products).ToList();
		}
	}


	
}
