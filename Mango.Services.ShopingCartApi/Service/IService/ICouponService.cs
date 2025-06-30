using Mango.Services.ShopingCartApi.Models.Dto;

namespace Mango.Services.ShopingCartApi.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponDetails(string couponCode);
    }
}
