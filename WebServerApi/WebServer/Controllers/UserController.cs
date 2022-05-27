using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Dto;
using WebServer.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebServer.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		//http://localhost:61491/api/user/registration
		[HttpPost]
		[Route("registration")]
		public IActionResult Registration(UserDto user)
		{
			UserDto retValue = _userService.Register(user);
			return Ok(retValue);
		}
		[HttpPost]
		[Route("login")]
		public IActionResult Login(UserDto user)
		{
			return Ok(_userService.Login(user));
		}


		[HttpPost]
		[Route("updateProfile")]
		public IActionResult UpdateProfile([FromBody]UserDto user )
		{	
			return Ok(_userService.UpdateProfile(user, Request.Headers));
		}

		[HttpGet]
		[Route("getProfile")]
		public IActionResult GetProfile(string token)
		{
			return Ok(_userService.GetProfile(token));
		}

		[HttpGet]
		[Route("checkdeliverstatus")]
		public IActionResult CheckDeliverStatus(string token)
		{
			return Ok(_userService.CheckDeliverStatus(token));
		}



		[HttpPost]
		[Route("verificateuser")]
		[Authorize(Roles = "administrator")]
		public IActionResult VerificateUser(string accountStatus,int id)
		{
			return Ok(_userService.VerificateUser(accountStatus,id));
		}
	
		[HttpGet]
		[Route("getusers")]
		[Authorize(Roles = "administrator")]
		public IActionResult GetUsers()
		{
			return Ok(_userService.GetUsers());
		}

	}
}
