using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

       
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [Authorize]
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto> lst = null;
            var response = await _couponService.GetAllCouponAsync();
            if(response !=null && response.IsSuccess)
            {
                lst = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(lst);
        }


        [HttpGet]
        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto model)
        {
            if(ModelState.IsValid)
            {
                var response = await _couponService.CreateCouponAsync(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CouponDelete(int coupanId)
        {
			var response = await _couponService.GetCouponByIdAsync(coupanId);
			if (response != null && response.IsSuccess)
			{
				var result = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
				return View(result);
			}
            else
            {
                TempData["error"] = response?.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto coupan)
        {
            var response = await _couponService.DeleteCouponAsync(coupan.CouponId);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CouponIndex)); ;
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(coupan);
        }
    }
}
