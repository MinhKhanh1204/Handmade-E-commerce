using Microsoft.AspNetCore.Mvc;

namespace HandicraftShop_Prodject.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
    }
}
