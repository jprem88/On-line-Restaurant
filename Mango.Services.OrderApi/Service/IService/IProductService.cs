

using Mango.Services.OrderApi.Models.Dto;

namespace Mango.Services.OrderApi.Service.IService
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProducts();
    }
}
