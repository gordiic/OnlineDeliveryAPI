using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Dto
{
	public class ProductDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Ingredients { get; set; }
		public double Price { get; set; }
		

		public ProductDto()
		{

		}

		public ProductDto(int id, string name, string ingredients, double price)
		{
			this.Id = id;
			this.Name = name;
			this.Ingredients = ingredients;
			this.Price = price;
		}

	
	}
}
