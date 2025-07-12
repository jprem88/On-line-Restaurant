
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
        private readonly IOrderService _orderService;
        public ShopingCartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            CartDto cartDto = await GetCartList();
            return View(cartDto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            CartDto cartDto = await GetCartList();
            return View(cartDto);
        }

        [HttpPost]
        //[ActionName("Checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            var cart = await GetCartList();
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            var response = await _orderService.CreateOrderAsync(cart);
            var order = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
            if(response !=null && response.IsSuccess)
            {
                //////////  stripe logic
                var domain = Request.Scheme + "://" + Request.Host.Value+"/";
                StripeRequestDto stripeRequestDto = new()
                {
                    ApproveUrl = domain + "cart/Confirmation?orderId=" + order.OrderHeaderId,
                    CancelUrl = domain + "cart/checkout",
                    OrderHeader = order
                };
                var stripeResponse = await _orderService.CreateStripeSessionAsync(stripeRequestDto);
                StripeRequestDto stripeResponseResult = JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Result));
                Response.Headers.Add("Location", stripeResponseResult.StripeSessionUrl);
                return new StatusCodeResult(303);
            }
            return View();

          
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            
            return View(orderId);
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
