using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using Mango.Web.Utlility;
using static Mango.Web.Utlility.SD;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateCouponAsync(CouponDto request)
        {

            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Post,
                Data =request,
                Url = SD.CouponApiBase + "api/coupon"
            });
        }

        public async Task<ResponseDto?> DeleteCouponAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Delete,
                Url = SD.CouponApiBase + "api/coupon/" + couponId
            });
        }

        public async Task<ResponseDto?> GetAllCouponAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Get,
                Url = SD.CouponApiBase+ "api/coupon"
            });
        }

        public async Task<ResponseDto?> GetCouponByCodeAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Get,
                Url = SD.CouponApiBase + "api/coupon/getbyCode/" + couponCode
            });
        }

        public async Task<ResponseDto?> GetCouponByIdAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Get,
                Url = SD.CouponApiBase + "api/coupon/" + couponId
            });
        }

        public async Task<ResponseDto?> UpdateCouponAsync(CouponDto request)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Put,
                Data = request,
                Url = SD.CouponApiBase + "api/coupon"
            });
        }
    }
}
