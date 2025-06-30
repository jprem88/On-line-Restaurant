using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Service.IService
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetAllCouponAsync();
        Task<ResponseDto?> GetCouponByCodeAsync(string couponCode);
        Task<ResponseDto?> GetCouponByIdAsync(int couponId);
        Task<ResponseDto?> CreateCouponAsync(CouponDto request);
        Task<ResponseDto?> UpdateCouponAsync(CouponDto request);
        Task<ResponseDto?> DeleteCouponAsync(int couponId);
    }
}
 