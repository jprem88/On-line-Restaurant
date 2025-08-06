using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utlility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult OrderIndex()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OrderDetails(int orderId)
        {
            return View();
        }

        [HttpGet]

        public IActionResult GetAllOrder()
        {
            IEnumerable<OrderHeaderDto> list = new List<OrderHeaderDto>();
            string userId = "";
            if(!User.IsInRole(SD.RoleAdmin))
            {
                userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            ResponseDto responseDto = _orderService.GetAllOrder(userId).GetAwaiter().GetResult();
            if(responseDto !=null && responseDto.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(Convert.ToString(responseDto.Result));
            }
            return Json(new { data = list });
        }
    }
}
