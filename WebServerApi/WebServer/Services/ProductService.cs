using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebServer.DB;
using WebServer.Dto;
using WebServer.Interfaces;
using WebServer.Models;

namespace WebServer.Services
{
	public class ProductService : IProductService
	{
		private readonly IMapper _mapper;
		private readonly DataBaseUserContext _dbContext;
		private readonly IConfigurationSection _secretKey;

		public ProductService(IMapper mapper, DataBaseUserContext dbContext, Microsoft.Extensions.Configuration.IConfiguration config)
		{
			_mapper = mapper;
			_dbContext = dbContext;
			_secretKey = config.GetSection("SecretKey");
		}
		public IEnumerable<ProductDto> GetProducts(IHeaderDictionary headers)
		{
			return _mapper.Map<List<ProductDto>>(_dbContext.products);
		}
		public ProductDto AddProduct(ProductDto product, IHeaderDictionary headers)
		{
			Product newProduct = _mapper.Map<Product>(product);
			//var handler = new JwtSecurityTokenHandler();
			//string authHeader = headers["Authorization"];
			//authHeader = authHeader.Replace("Bearer ", "");
			//var jsonToken = handler.ReadToken(authHeader);
			//var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
			//var stringId = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
			//int id = int.Parse(stringId);

			EntityEntry ee = _dbContext.products.Add(newProduct);
			_dbContext.SaveChanges();

			return _mapper.Map<ProductDto>(ee.Entity); ;
		}

	}
}
