using Mango.Web.Models.Dto;
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDto?> GetAllProductAsync();
        Task<ResponseDto?> GetProductByIdAsync(int couponId);
        Task<ResponseDto?> CreateProductAsync(ProductDto request);
        Task<ResponseDto?> UpdateProductAsync(ProductDto request);
        Task<ResponseDto?> DeleteProductAsync(int productId);
    }
}
