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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebServer.DB;
using WebServer.Dto;
using WebServer.Interfaces;
using WebServer.Models;

namespace WebServer.Services
{
	public class UserService : IUserService
	{
        private readonly IMapper _mapper;
        private readonly DataBaseUserContext _dbContext;
		private readonly IConfigurationSection _secretKey;

		public UserService(IMapper mapper, DataBaseUserContext dbContext, Microsoft.Extensions.Configuration.IConfiguration config)
		{
            _mapper = mapper;
            _dbContext = dbContext;
			_secretKey = config.GetSection("SecretKey");
        }

		

        public UserDto Register(UserDto user)
		{
            User newUser = _mapper.Map<User>(user);
            if(String.IsNullOrWhiteSpace(newUser.Password) || String.IsNullOrWhiteSpace(newUser.UserName) || String.IsNullOrWhiteSpace(newUser.LastName) ||
                String.IsNullOrWhiteSpace(newUser.Name) || String.IsNullOrWhiteSpace(newUser.UserType.ToString()) || String.IsNullOrWhiteSpace(newUser.AccountStatus.ToString()))
			{
                if(newUser.AccountStatus == Models.AccountStatus.declined.ToString() || newUser.Password.Length<7 || !newUser.Email.Contains('@') || !newUser.Email.Contains('.'))
				{
                    return null;
				}
			}
			//byte[] niz = BCrypt.Generate(BCrypt.PasswordToByteArray(newUser.Password.ToCharArray()), BCrypt.PasswordToByteArray(_secretKey.ToString().ToCharArray()),16);
			newUser.Password= BCrypt.Net.BCrypt.HashPassword(newUser.Password);
			EntityEntry s = _dbContext.users.Add(newUser);
			try{
				_dbContext.SaveChanges();
			}catch(Exception e)
			{
				return null;
			}
            return _mapper.Map<UserDto>(newUser);
		}

		public TokenDto Login(UserDto user)
		{
			TokenDto token = new TokenDto();
			User newUser = _mapper.Map<User>(user);
			string hashedPassword = newUser.Password;//treba hesovati ili na frontu treba hash
			User registredUser = _dbContext.users.ToList().FindLast(x => newUser.UserName==x.UserName);
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

			if (String.IsNullOrWhiteSpace(newUser.Password) || String.IsNullOrWhiteSpace(newUser.UserName) || String.IsNullOrWhiteSpace(newUser.LastName) ||
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
			try
			{
				//_dbContext.users.Update(newUser);
				//var result = _dbContext.users.Where(u => u.Id == id).ToList();
				EntityEntry ee = _dbContext.users.Update(newUser);
				_dbContext.SaveChanges();
				return _mapper.Map<UserDto>(ee.Entity);

			}
			catch (Exception e)
			{
				return null;
			}
		}

		public UserDto GetProfile(string token)
		{

			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(token);
			var tokenS = handler.ReadToken(token) as JwtSecurityToken;
			var stringId = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
			int id = int.Parse(stringId);
			
			try
			{
				var result = _dbContext.users.Where(u => u.Id == id).ToList();
				return _mapper.Map < UserDto > (result[0]);
			}catch(Exception e)
			{
				return null;
			}
		}
	}
}
