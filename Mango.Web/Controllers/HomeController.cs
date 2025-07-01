using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        } 

        public async Task<IActionResult> Index()
        {
            List<ProductDto> lst = null;
            var response = await _productService.GetAllProductAsync();
            if (response != null && response.IsSuccess) 
            {
                lst = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(lst);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            var response = await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSuccess)
            {
                var result = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(result);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> Details(ProductDto product)
        {
            CartDto cartDto = new CartDto()
            {
                CartHeader = new()
                {
                    UserId = User.Claims.Where(x => x.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };
              
            CartDetailDto cartDetailDto = new()
            {
                ProductId = product.ProductId,
                Count = product.Count,
            };

            List<CartDetailDto> cartDetailDtos = new List<CartDetailDto>() { cartDetailDto };
            cartDto.CartDetails = cartDetailDtos;

            var response = await _cartService.UpsertItem(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "item has been added to shopping cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}