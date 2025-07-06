using AutoMapper;
using Mango.Services.ShopingCartApi.Models;
using Mango.Services.ShopingCartApi.Models.Dto;

namespace Mango.Services.ShopingCartApi
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();  
                config.CreateMap<CartDetailDto, CartDetail>().ReverseMap();
            });
            return mapperConfig;
        }
    }
}
