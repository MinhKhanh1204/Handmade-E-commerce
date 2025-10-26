using Microsoft.AspNetCore.Mvc;
using Services;

namespace HandicraftShop_Prodject.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var account = _accountService.Login(email, password);

            if (account != null)
            {
                HttpContext.Session.SetString("AccountId", account.AccountId.ToString());
                HttpContext.Session.SetString("Username", account.Username);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
