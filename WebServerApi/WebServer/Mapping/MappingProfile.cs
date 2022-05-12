using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AutoMapper;
using WebServer.Dto;
using WebServer.Models;

namespace WebServer.Mapping
{
	public class MappingProfile : Profile
	{
        public MappingProfile()
        {
			CreateMap<User, UserDto>().ReverseMap(); //Kazemo mu da mapira Subject na SubjectDto i obrnuto
			CreateMap<Order, OrderDto>().ReverseMap();
			CreateMap<Product, ProductDto>().ReverseMap();
			CreateMap<OrderProduct, OrderProductDto>().ReverseMap();
			CreateMap<Token, TokenDto>().ReverseMap();
			CreateMap<List<Product>, List<ProductDto>>().ReverseMap();
			CreateMap<List<Order>, List<OrderDto>>().ReverseMap();

		}
	}
}
