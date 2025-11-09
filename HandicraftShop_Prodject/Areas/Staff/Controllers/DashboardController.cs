using Microsoft.AspNetCore.Mvc;

namespace HandicraftShop_Prodject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
