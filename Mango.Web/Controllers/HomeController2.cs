using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class HomeController2 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
