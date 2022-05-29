using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.DB.Providers
{
	public class UserProvider
	{
		private readonly DataBaseUserContext _dbContext;
		public UserProvider(DataBaseUserContext dbContext)
		{
			_dbContext = dbContext;
		}

		public User AddUser(User newUser)
		{
			EntityEntry ee = _dbContext.users.Add(newUser);
			try
			{
				_dbContext.SaveChanges();
			}
			catch (Exception e)
			{
				return null;
			}
			return (User)ee.Entity;
		}

		public User LoginUser(User newUser)
		{
			return _dbContext.users.ToList().FindLast(x => newUser.UserName == x.UserName);
		}

		public User UpdateUser(User newUser)
		{
			try
			{
				EntityEntry ee = _dbContext.users.Update(newUser);
				_dbContext.SaveChanges();
				return (User)ee.Entity;
			}catch(Exception e)
			{
				return null;
			}
		}

		public User GetUser(int id)
		{
			try
			{
				var result = _dbContext.users.Where(u => u.Id == id).ToList();			
				return result[0];
			}
			catch(Exception e)
			{
				return null;
			}
		}

		public IEnumerable<Order> ChechUserDeliveryStatus(int id)
		{
			return _dbContext.orders.Where(o => (o.DelivererId == id || o.UserId == id)).ToList();		
		}

		public User VerificateUser(string accountStatus, int id)
		{

			User u = _dbContext.users.Find(id);
			u.AccountStatus = accountStatus;
			_dbContext.users.Update(u);
			_dbContext.SaveChanges();

			return u;
		}

		public List<User> GetUsers()
		{
			List<User> users = _dbContext.users.ToList();
			return users;
		}
	}
}
