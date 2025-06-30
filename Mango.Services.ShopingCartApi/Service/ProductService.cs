using Mango.Services.ShopingCartApi.Models.Dto;
using Mango.Services.ShopingCartApi.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShopingCartApi.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<List<ProductDto>> GetProducts()
        {
           var httpClient = _httpClientFactory.CreateClient("Product");
            var request = await httpClient.GetAsync($"api/product");
            var apiContent =  await request.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (response.IsSuccess) {

                var product = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
                return product;
            
            }
            return new List<ProductDto>(); ;
        }
    }
}
