using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	public class User
	{
		int id;
		string userName;
		string email;
		string password;
		string name;
		string lastName;
		string birthDate;
		string address;
		string userType;

		public User()
		{

		}

		public User(int id, string userName, string email, string password, string name, string lastName, string birthDate, string address, string userType)
		{
			this.id = id;
			this.userName = userName;
			this.email = email;
			this.password = password;
			this.name = name;
			this.lastName = lastName;
			this.birthDate = birthDate;
			this.address = address;
			this.userType = userType;
		}

		public int Id { get => id; set => id = value; }
		public string UserName { get => userName; set => userName = value; }
		public string Email { get => email; set => email = value; }
		public string Password { get => password; set => password = value; }
		public string Name { get => name; set => name = value; }
		public string LastName { get => lastName; set => lastName = value; }
		public string BirthDate { get => birthDate; set => birthDate = value; }
		public string Address { get => address; set => address = value; }
		public string UserType { get => userType; set => userType = value; }
	}
}
