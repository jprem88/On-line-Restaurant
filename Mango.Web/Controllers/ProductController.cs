using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
      
        public async Task<IActionResult> ProductIndex()
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


        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProductAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "product created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int productId)
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

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(ProductDto product)
        {
            var response = await _productService.DeleteProductAsync(product.ProductId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "product deleted successfully";
                return RedirectToAction(nameof(ProductIndex)); ;
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int productId)
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

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductDto product)
        {
            var response = await _productService.UpdateProductAsync(product);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "product updated successfully";
                return RedirectToAction(nameof(ProductIndex)); ;
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(product);
        }
    }
}
