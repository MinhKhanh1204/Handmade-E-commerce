using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandicraftShop_Prodject.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
