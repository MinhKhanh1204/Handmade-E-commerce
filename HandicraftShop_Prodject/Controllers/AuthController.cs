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
					// Cấu hình thời gian cookie dựa vào RememberMe
					var authProperties = new AuthenticationProperties
					{
						IsPersistent = model.Login.RememberMe,
						ExpiresUtc = model.Login.RememberMe 
							? DateTimeOffset.UtcNow.AddDays(30)  // Remember for 30 days
							: DateTimeOffset.UtcNow.AddHours(2)  // Session only (2 hours)
					};

					//Thiết lập phiên đăng nhập cho tài khoản
					await HttpContext.SignInAsync(
						CookieAuthenticationDefaults.AuthenticationScheme,
						AccountUtils.CreatePrincipal(account),
						authProperties
					);

					var firstRole = account.UserRoles.FirstOrDefault()?.Role?.RoleName;
					switch (firstRole)
					{
						case "Admin":
                            HttpContext.Session.SetString("StaffId", account.AccountId);
                            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
						case "Employee":
                            HttpContext.Session.SetString("StaffId", account.AccountId);
                            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
						default:
                            HttpContext.Session.SetString("CustomerId", account.AccountId);
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
            // Bỏ validate Login
            foreach (var key in ModelState.Keys
                                     .Where(k => k.StartsWith("Login."))
                                     .ToList())
            {
                ModelState.Remove(key);
            }

            // Validate Register
            ModelState.ClearValidationState(nameof(model.Login));
            TryValidateModel(model.Register, nameof(model.Register));

            var registerDto = model.Register;

            if (ModelState.IsValid)
            {
                if (_accountService.GetAccountByEmail(registerDto.Email) != null)
                {
                    ModelState.AddModelError("Register.Email", "Email already exists.");
                    return View("Login", model);
                }

                var success = _accountService.Register(registerDto);
                if (success)
                {
                    TempData["Success"] = "Account created successfully! Please log in.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("Register.Email", "Failed to create account.");
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
                    HttpContext.Session.SetString("StaffId", account.AccountId);
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
				case "Employee":
                    HttpContext.Session.SetString("StaffId", account.AccountId);
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
				default:
                    HttpContext.Session.SetString("CustomerId", account.AccountId);
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
                    HttpContext.Session.SetString("StaffId", account.AccountId);
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
				case "Employee":
                    HttpContext.Session.SetString("StaffId", account.AccountId);
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
				default:
                    HttpContext.Session.SetString("CustomerId", account.AccountId);
                    return RedirectToAction("Index", "Home");
			}
		}


		// GET: ForgotPassword
		public IActionResult ForgotPassword()
		{
			return View();
		}

		// POST: ForgotPassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
		{
			if (ModelState.IsValid)
			{
				var result = await _accountService.ForgotPasswordAsync(model);

				if (result)
				{
					TempData["success"] = "A reset code has been sent to your email.";
					return RedirectToAction("ResetPassword", new { email = model.Email });
				}
				else
				{
					TempData["error"] = "Email not found or failed to send reset code.";
				}
			}

			return View(model);
		}

		// GET: ResetPassword
		public IActionResult ResetPassword(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return RedirectToAction("ForgotPassword");
			}

			var model = new ResetPasswordDTO { Email = email };
			return View(model);
		}

		// POST: ResetPassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ResetPassword(ResetPasswordDTO model)
		{
			if (ModelState.IsValid)
			{
				var result = _accountService.ResetPassword(model);

				if (result)
				{
					TempData["success"] = "Password has been reset successfully! Please login.";
					return RedirectToAction("Login");
				}
				else
				{
					TempData["error"] = "Invalid or expired reset code.";
				}
			}

			return View(model);
		}
	}
}
