using Mango.Services.ShopingCartApi.Models.Dto;
using Mango.Services.ShopingCartApi.Service.IService;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Mango.Services.ShopingCartApi.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CouponDto> GetCouponDetails(string couponCode)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("Coupon");
            var response = await httpClient.GetAsync($"api/coupon/getbyCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if(result.IsSuccess)
            {
                var Coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(result.Result));
                return Coupon;
            }
            return new CouponDto();
        }
    }
}
