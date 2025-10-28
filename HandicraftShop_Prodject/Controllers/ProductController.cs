using Microsoft.AspNetCore.Mvc;

namespace HandicraftShop_Prodject.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
