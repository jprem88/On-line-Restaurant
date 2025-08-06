using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrderAsync(CartDto request);
        Task<ResponseDto?> CreateStripeSessionAsync(StripeRequestDto request);
        Task<ResponseDto?> ValidateStripeSessionAsync(int orderHeaderId);
        Task<ResponseDto?> GetAllOrder(string? userId);
        Task<ResponseDto?> GetOrder(int  orderId);
        Task<ResponseDto?> UpdateOrderStatus(int  orderId,string status);


    }
}
 