using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Dto;

namespace WebServer.Interfaces
{
	public interface IUserService
	{
		UserDto Register(UserDto user);
		TokenDto Login(UserDto user);
		bool UpdateProfile(UserDto user);

	}
}
