using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Dto;

namespace WebServer.Interfaces
{
	public interface IProductService
	{
		IEnumerable<ProductDto> GetProducts(IHeaderDictionary headers);
		ProductDto AddProduct(ProductDto product, IHeaderDictionary headers);
	}
}
