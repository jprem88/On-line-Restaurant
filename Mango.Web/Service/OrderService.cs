using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using Mango.Web.Utlility;
using static Mango.Web.Utlility.SD;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;

        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateOrderAsync(CartDto request)
        {

            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Post,
                Data =request,
                Url = SD.OrderApiBase + "api/order/createorder"
            });
        }

        public async Task<ResponseDto?> CreateStripeSessionAsync(StripeRequestDto request)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Post,
                Data = request,
                Url = SD.OrderApiBase + "api/order/createstripesession"
            });
        }
    }
}
