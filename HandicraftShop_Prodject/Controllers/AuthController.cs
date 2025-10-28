using Microsoft.AspNetCore.Mvc;
using Services;
using DTO;
using HandicraftShop_Prodject.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using HandicraftShop_Prodject.Utils;

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
		public async Task<IActionResult> LoginAsync(AuthViewModel model)
		{
			// Xóa lỗi liên quan đến Register
			foreach (var key in ModelState.Keys
										 .Where(k => k.StartsWith("Register."))
										 .ToList())
			{
				ModelState.Remove(key);
			}

			ModelState.ClearValidationState(nameof(model.Register));
			TryValidateModel(model.Login, nameof(model.Login));
			if (ModelState.IsValid)
			{
				var account = _accountService.Login(model.Login);

				if (account != null)
				{
					//Thiết lập phiên đăng nhập cho tài khoản
					await HttpContext.SignInAsync(AccountUtils.CreatePrincipal(account));
					var firstRole = account.UserRoles.FirstOrDefault()?.Role?.RoleName;
					switch (firstRole)
					{
						case "Admin":
							return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
						case "Employee":
							return RedirectToAction("Index", "Dashboard", new { area = "Employee" });
						default:
							return RedirectToAction("Index", "Home");
					}
				}

				ViewBag.Error = "Invalid email or password.";
			}
			return View(model);
		}

		[HttpPost]
		public IActionResult Register(AuthViewModel model)
		{
			foreach (var key in ModelState.Keys
								 .Where(k => k.StartsWith("Login."))
								 .ToList())
			{
				ModelState.Remove(key);
			}
			// Chỉ validate phần Register, bỏ qua Login
			ModelState.ClearValidationState(nameof(model.Login));
			TryValidateModel(model.Register, nameof(model.Register));
			var registerDto = model.Register;
			if (ModelState.IsValid)
			{
				var success = _accountService.Register(registerDto);
				if (success)
				{
					TempData["Success"] = "Account created successfully! Please log in.";
					return RedirectToAction("Login");
				}

				ModelState.AddModelError("Email", "Email already exists.");
			}
			return View("Login", model);
		}

		public async Task<IActionResult> LogoutAsync()
		{
			HttpContext.Session.Clear();
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login");
		}
		public IActionResult AccessDenied()
		{
			return View();
		}
		
	}
}
