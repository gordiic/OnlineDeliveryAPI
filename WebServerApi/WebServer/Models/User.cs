using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Models
{
	[Table("Users", Schema = "dbo")]
	public class User
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Name { get; set; }
		public string LastName { get; set; }
		public string BirthDate { get; set; }
		public string Address { get; set; }
		public string UserType { get; set; }
		public string AccountStatus { get; set; }
		public ICollection<Order> Orders { get; set; }
		public byte[] Image { get; set; }


		public User()
		{

		}

		public User(int id, string userName, string email, string password, string name, string lastName, string birthDate, string address, string userType, string accountStatus, string image)
		{
			this.Id = id;
			this.UserName = userName;
			this.Email = email;
			this.Password = password;
			this.Name = name;
			this.LastName = lastName;
			this.BirthDate = birthDate;
			this.Address = address;
			//this.UserType = (UserType)Enum.Parse(typeof(UserType), userType);
			//this.AccountStatus = (AccountStatus)Enum.Parse(typeof(AccountStatus), accountStatus);
			this.UserType = userType;
			this.AccountStatus = accountStatus;
			this.Image = Encoding.ASCII.GetBytes(image);
		}
	}
}
