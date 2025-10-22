using Microsoft.AspNetCore.Mvc;

namespace HandicraftShop_Prodject.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
