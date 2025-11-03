using Microsoft.AspNetCore.Mvc;
using Services;
using DTO;
using HandicraftShop_Prodject.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using HandicraftShop_Prodject.Utils;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Facebook;

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

                TempData["error"] = "Invalid email or password.";
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

		public async Task LoginGoogle()
		{
			await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
				new AuthenticationProperties
				{
					RedirectUri = Url.Action("GoogleResponse")
				});
		}

		public async Task<IActionResult> GoogleResponse()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			var claims = result.Principal.Identities.FirstOrDefault().Claims;
			// Lấy thông tin từ Google
			var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

			var account = _accountService.GetAccountByEmail(email);

			if (account == null)
			{
				//register account
				var success = _accountService.Register(new RegisterDTO
				{
					Username = name,
					Email = email,
					Password = "123456",
					FullName = name
				});

                account = _accountService.GetAccountByEmail(email);
            }

            //login
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

		public async Task LoginFacebook()
		{
			await HttpContext.ChallengeAsync(
				FacebookDefaults.AuthenticationScheme,
				new AuthenticationProperties { RedirectUri = Url.Action("FacebookResponse") }
			);
		}

		public async Task<IActionResult> FacebookResponse()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			if (!result.Succeeded)
				return RedirectToAction("Login");

			var claims = result.Principal.Identities.FirstOrDefault().Claims;

			var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

			var account = _accountService.GetAccountByEmail(email);

			if (account == null)
			{
				//register account
				var success = _accountService.Register(new RegisterDTO
				{
					Username = name,
					Email = email,
					Password = "123456",
					FullName = name
				});
                account = _accountService.GetAccountByEmail(email);
            }

			//login
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


	}
}
