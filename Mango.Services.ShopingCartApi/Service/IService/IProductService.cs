using Mango.Services.ShopingCartApi.Models.Dto;

namespace Mango.Services.ShopingCartApi.Service.IService
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProducts();
    }
}
