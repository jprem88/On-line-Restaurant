
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class ShopingCartController : Controller
    {
        private readonly ICartService _cartService;
        public ShopingCartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            CartDto cartDto = await GetCartList();
            return View(cartDto);
        }

        public async Task<IActionResult> Remove(string cartDetailsId)
        {
            var response = await _cartService.RemoveItem(cartDetailsId);
            if (response != null && response.IsSuccess) {
                TempData["success"] = "Item removed sucessfully";
                return RedirectToAction(nameof(CartIndex));

            }
            else
            {
                TempData["error"] = "some thing went wrong";
                return RedirectToAction(nameof(CartIndex));
            }

        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cart)
        {
            var response = await _cartService.ApplyRemoveCoupon(cart);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon applied sucessfully";
                return RedirectToAction(nameof(CartIndex));

            }
            else
            {
                TempData["error"] = "some thing went wrong";
                return RedirectToAction(nameof(CartIndex));
            }

        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cart)
        {
            cart.CartHeader.CouponCode = "";
            var response = await _cartService.ApplyRemoveCoupon(cart);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon removed sucessfully";
                return RedirectToAction(nameof(CartIndex));

            }
            else
            {
                TempData["error"] = "some thing went wrong";
                return RedirectToAction(nameof(CartIndex));
            }

        }

        private async Task<CartDto> GetCartList()
        {
            string userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            var response = await _cartService.GetCartList(userId);
            if (response != null && response.IsSuccess)
            {
                var cartData = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartData;
            }
            return new CartDto();  
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cart)
        {
            CartDto cartDto = await GetCartList();
            cartDto.CartHeader.Email = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Email).FirstOrDefault()?.Value;
            var response = await _cartService.EmailCart(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Email will processed and send shortly";
                return RedirectToAction(nameof(CartIndex));

            }
            else
            {
                TempData["error"] = "some thing went wrong";
                return RedirectToAction(nameof(CartIndex));
            }

        }


    }
}
