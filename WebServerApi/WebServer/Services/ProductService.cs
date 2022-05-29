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
using WebServer.DB.Providers;
using WebServer.Dto;
using WebServer.Interfaces;
using WebServer.Mapping;
using WebServer.Models;

namespace WebServer.Services
{
	public class ProductService : IProductService
	{
		private readonly IMapper _mapper;
		private readonly DataBaseUserContext _dbContext;
		private readonly IConfigurationSection _secretKey;
		private readonly ProductProvider _productProvider;
		private readonly MyMapper _myMapper;



		public ProductService(IMapper mapper, DataBaseUserContext dbContext, Microsoft.Extensions.Configuration.IConfiguration config)
		{
			_mapper = mapper;
			_dbContext = dbContext;
			_secretKey = config.GetSection("SecretKey");
			_productProvider = new ProductProvider(dbContext);
			_myMapper = new MyMapper();

		}
		public IEnumerable<ProductDto> GetProducts(IHeaderDictionary headers)
		{
			List<Product> products = _productProvider.GetProducts();
			
			return _myMapper.MapProductToProductDto(products);
		}
		public ProductDto AddProduct(ProductDto product, IHeaderDictionary headers)
		{
			Product newProduct = _mapper.Map<Product>(product);
			Product ret = _productProvider.AddProduct(newProduct);
			return _mapper.Map<ProductDto>(ret); 
		}

	}
}
