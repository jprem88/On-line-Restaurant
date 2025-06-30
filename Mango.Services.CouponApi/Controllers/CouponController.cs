using AutoMapper;
using Mango.Services.CouponApi.Data;
using Mango.Services.CouponApi.Models;
using Mango.Services.CouponApi.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponApi.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private ResponseDto responseDto;
        private readonly IMapper _mapper;
        public CouponController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            responseDto = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                var result = _appDbContext.Coupons.ToList();
                var mappedresult =_mapper.Map<List<CouponDto>>(result);
                responseDto.Result = mappedresult;
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                var result = _appDbContext.Coupons.FirstOrDefault(x=>x.CouponId ==id);

                responseDto.Result = _mapper.Map<CouponDto>(result);
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }

        [HttpGet]
        [Route("getbyCode/{couponCode}")]
        public ResponseDto getbyCode(string couponCode)
        {
            try
            {
                var result = _appDbContext.Coupons.FirstOrDefault(x => x.CouponCode.ToLower() == couponCode.ToLower());

                responseDto.Result = _mapper.Map<CouponDto>(result);
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }

        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ResponseDto Post([FromBody] CouponDto coupon)
        {
            try
            {
                var convertedCoupon = _mapper.Map<Coupon>(coupon);
                _appDbContext.Coupons.Add(convertedCoupon);
                _appDbContext.SaveChanges();
                responseDto.Result = _mapper.Map<CouponDto>(convertedCoupon);

            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }

        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto coupon)
        {
            try
            {
                var convertedCoupon = _mapper.Map<Coupon>(coupon);
                _appDbContext.Coupons.Update(convertedCoupon);
                _appDbContext.SaveChanges();
                responseDto.Result = _mapper.Map<CouponDto>(convertedCoupon);

            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }

        [HttpDelete]
		[Authorize(Roles = "ADMIN")]
		[Route("{id:int}")]

        public ResponseDto Delete(int id)
        {
            try
            {
                var result = _appDbContext.Coupons.First(x => x.CouponId == id);
                _appDbContext.Coupons.Remove(result);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }
    }
}
