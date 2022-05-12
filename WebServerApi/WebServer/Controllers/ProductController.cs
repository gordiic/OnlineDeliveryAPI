using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Dto;
using WebServer.Interfaces;

namespace WebServer.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductController(IProductService productService)
		{
			_productService = productService;
		}

		[HttpGet]
		[Route("getproducts")]
		public IActionResult GetProducts()
		{
			return Ok(_productService.GetProducts(Request.Headers));
		}

		[HttpPost]
		[Route("addproduct")]
		//[Authorize(Roles="administrator")]
		public IActionResult AddProduct(ProductDto product)
		{
			return Ok(_productService.AddProduct(product, Request.Headers));
		}
	}
}
