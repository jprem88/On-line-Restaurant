using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using Mango.Web.Utlility;
using static Mango.Web.Utlility.SD;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateProductAsync(ProductDto request)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Post,
                Data = request,
                Url = SD.ProductApiBase + "api/product"
            },true);
        }

        public async Task<ResponseDto?> DeleteProductAsync(int productId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Delete,
                Url = SD.ProductApiBase + "api/product/" + productId
            },true);
        }

        public async Task<ResponseDto?> GetAllProductAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Get,
                Url = SD.ProductApiBase + "api/product"
            },true);
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int productId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Get,
                Url = SD.ProductApiBase + "api/product/" + productId
            });
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto request)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Put,
                Data = request,
                Url = SD.ProductApiBase + "api/product"
            });
        }
    }
}
