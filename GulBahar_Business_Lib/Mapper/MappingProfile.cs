using AutoMapper;
using GulBahar_DataAcess_Lib;
using GulBahar_DataAcess_Lib.ViewModel;
using GulBahar_Models_Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GulBahar_Business_Lib.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
			CreateMap<Product, ProductDTO>().ReverseMap();
			CreateMap<ProductPrice, ProductPriceDTO>().ReverseMap();
            CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
        }
    }
} 
