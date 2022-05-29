using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebServer.DB;
using WebServer.DB.Providers;
using WebServer.Dto;
using WebServer.Interfaces;
using WebServer.Mapping;
using WebServer.Models;

namespace WebServer.Services
{
	public class UserService : IUserService
	{
        private readonly IMapper _mapper;
        private readonly DataBaseUserContext _dbContext;
		private readonly IConfigurationSection _secretKey;
		private readonly MyMapper _mymapper;
		private readonly UserProvider _userProvider;

		public UserService(IMapper mapper, DataBaseUserContext dbContext, Microsoft.Extensions.Configuration.IConfiguration config)
		{
            _mapper = mapper;
            _dbContext = dbContext;
			_secretKey = config.GetSection("SecretKey");
			_mymapper = new MyMapper();
			_userProvider = new UserProvider(dbContext);
        }

		

        public UserDto Register(UserDto user)
		{
            User newUser = _mapper.Map<User>(user);
			newUser.Image = Encoding.ASCII.GetBytes(user.Image);
			if (String.IsNullOrWhiteSpace(newUser.Password) || newUser.Image.Length==0 || String.IsNullOrWhiteSpace(newUser.UserName) || String.IsNullOrWhiteSpace(newUser.LastName) ||
                String.IsNullOrWhiteSpace(newUser.Name) || String.IsNullOrWhiteSpace(newUser.UserType.ToString()) || String.IsNullOrWhiteSpace(newUser.AccountStatus.ToString()))
			{
                if(newUser.AccountStatus == Models.AccountStatus.declined.ToString() || newUser.Password.Length<7 || !newUser.Email.Contains('@') || !newUser.Email.Contains('.'))
				{
                    return null;
				}
			}
			//byte[] niz = BCrypt.Generate(BCrypt.PasswordToByteArray(newUser.Password.ToCharArray()), BCrypt.PasswordToByteArray(_secretKey.ToString().ToCharArray()),16);
			newUser.Password= BCrypt.Net.BCrypt.HashPassword(newUser.Password);

			if (_userProvider.AddUser(newUser) == null)
			{
				return null;
			}
			UserDto ret = _mapper.Map<UserDto>(newUser);
			ret.Image = Encoding.Default.GetString(newUser.Image);
			return ret;
		}

		public TokenDto Login(UserDto user)
		{
			TokenDto token = new TokenDto();
			User newUser = _mapper.Map<User>(user);
			string hashedPassword = newUser.Password;//treba hesovati ili na frontu treba hash
			User registredUser = _userProvider.LoginUser(newUser);
			if (registredUser!=null && BCrypt.Net.BCrypt.Verify(newUser.Password, registredUser.Password))
			{
				List<Claim> claims = new List<Claim>();
				claims.Add(new Claim(ClaimTypes.AuthorizationDecision, registredUser.AccountStatus));
				claims.Add(new Claim(ClaimTypes.Role, registredUser.UserType));
				claims.Add(new Claim(ClaimTypes.NameIdentifier, registredUser.Id.ToString()));

				SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey.Value));
				var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
				var tokeOptions = new JwtSecurityToken(
					issuer: "http://localhost:61491", //url servera koji je izdao token
					claims: claims, //claimovi
					expires: DateTime.Now.AddMinutes(20), //vazenje tokena u minutama
					signingCredentials: signinCredentials //kredencijali za potpis
				);
				token._Token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
				return token;
			}
			return null;
		}

		public UserDto UpdateProfile(UserDto user, IHeaderDictionary headers)
		{
			User newUser = _mapper.Map<User>(user);
			newUser.Image = Encoding.ASCII.GetBytes(user.Image);

			if (String.IsNullOrWhiteSpace(newUser.Password)|| newUser.Image.Length==0 || String.IsNullOrWhiteSpace(newUser.UserName) || String.IsNullOrWhiteSpace(newUser.LastName) ||
				String.IsNullOrWhiteSpace(newUser.Name) || String.IsNullOrWhiteSpace(newUser.UserType.ToString()) || String.IsNullOrWhiteSpace(newUser.AccountStatus.ToString()))
			{
				if (newUser.AccountStatus == Models.AccountStatus.declined.ToString() || newUser.Password.Length < 7 || !newUser.Email.Contains('@') || !newUser.Email.Contains('.'))
				{
					return null;
				}
			}
			var handler = new JwtSecurityTokenHandler();
			string authHeader = headers["Authorization"];
			authHeader = authHeader.Replace("Bearer ", "");
			var jsonToken = handler.ReadToken(authHeader);
			var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
			var stringId = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
			int id = int.Parse(stringId);
			newUser.Id = id;
			newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

			User ret = _userProvider.UpdateUser(newUser);
			return _mapper.Map<UserDto>(ret);
		}

		public UserDto GetProfile(string token)
		{

			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(token);
			var tokenS = handler.ReadToken(token) as JwtSecurityToken;
			var stringId = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
			int id = int.Parse(stringId);
	
			User ret = _userProvider.GetUser(id);
			if (ret == null)
			{
				return null;
			}
			UserDto ret2 = _mapper.Map<UserDto>(ret);
			ret2.Image = Encoding.Default.GetString(ret.Image);
			return ret2;
		}

		public OrderDto CheckDeliverStatus(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(token);
			var tokenS = handler.ReadToken(token) as JwtSecurityToken;
			var stringId = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
			int id = int.Parse(stringId);

			IEnumerable<Order> result  = _userProvider.ChechUserDeliveryStatus(id);
			if (result.ToList().Count==0)
			{
				return null;
			}
			DateTime time;
			DateTime now = DateTime.Now;
			foreach(Order o in result)
			{
				if (!string.IsNullOrEmpty(o.AcceptanceTime))
				{
					time = DateTime.Parse(o.AcceptanceTime);
					if (time.Day == now.Day)
					{
						if (time.Hour == now.Hour)
						{
							if ((now.Minute - time.Minute) < o.DeliverTime)
							{
								return _mapper.Map<OrderDto>(o);
							}
						}else if ((now.Hour - time.Hour) == 1)
						{
							if ((60 - time.Minute) + now.Minute < o.DeliverTime)
							{
								return _mapper.Map<OrderDto>(o);
							}
						}
					}
				}
			}
			return null;
		}

		public UserDto VerificateUser(string accountStatus,int id)
		{
			User u = _userProvider.VerificateUser(accountStatus,id);
			var smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential("nebojsagordic@gmail.com", "nebojsagordic99"),
				EnableSsl = true,
			};

			smtpClient.Send("nebojsagordic@gmail.com", "nebojsagordic@gmail.com", "verification", "Your account is "+accountStatus+".");
			if (u == null)
				return null;
			return _mapper.Map<UserDto>(u);
		}

		public List<UserDto> GetUsers()
		{
			List<User> users = _userProvider.GetUsers();
			List<UserDto> ret = _mymapper.MapUserToUserDto(users);
			return ret;
		}
	}
}
