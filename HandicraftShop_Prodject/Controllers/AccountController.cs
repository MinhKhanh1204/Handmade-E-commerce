using BussinessObject;
using DTO;
using HandicraftShop_Prodject.Models;
using HandicraftShop_Prodject.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;

namespace HandicraftShop_Prodject.Controllers
{
	[Authorize(Roles = "Customer")]
	public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public IActionResult Profile()
        {
            var userData = AccountUtils.GetUserData(User);
            var account = _accountService.GetAccountByID(userData.AccountId);
            if (account == null)
            {
                return NotFound();
            }
			var model = new ProfileViewModel
			{
				AccountId = account.AccountId,
				Username = account.Username,
				Avatar = account.Avatar,
				FullName = account.Customer.FullName,
				DateOfBirth = (DateOnly)account.Customer.DateOfBirth,
				Gender = account.Customer.Gender,
				Phone = account.Customer.Phone,
				Address = account.Customer.Address
			};
			return View(model);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Profile(ProfileViewModel account, IFormFile? Avatar)
		{
			var userData = AccountUtils.GetUserData(User);
			if (userData.AccountId != account.AccountId)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				try
				{
					var existingAccount = _accountService.GetAccountByID(userData.AccountId);
					if (existingAccount == null)
					{
						return NotFound();
					}

					// Update fields
					existingAccount.Username = account.Username;
					existingAccount.Customer.FullName = account.FullName;
					existingAccount.Customer.DateOfBirth = account.DateOfBirth;
					existingAccount.Customer.Gender = account.Gender;
					existingAccount.Customer.Phone = account.Phone;
					existingAccount.Customer.Address = account.Address;

					// Handle Avatar (image)
					if (Avatar != null && Avatar.Length > 0)
					{
						// Optional: Delete old image file from server if you store it physically

						// Save new image (example using wwwroot/uploads/)
						var fileName = Path.GetFileName(Avatar.FileName);
						var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/person", fileName);

						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await Avatar.CopyToAsync(stream);
						}

						// Save the relative path
						existingAccount.Avatar = fileName;
					}

					_accountService.UpdateProfile(existingAccount);
					await HttpContext.SignInAsync(AccountUtils.CreatePrincipal(existingAccount));
					TempData["success"] = "Update success";
					return RedirectToAction("Profile", "Account");
				}
				catch (Exception)
				{
					Console.WriteLine("s");
                }
			}
			return View(account);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ChangePassword(ChangePasswordDTO model)
		{
			if (ModelState.IsValid)
			{
				var userData = AccountUtils.GetUserData(User);
				var result = _accountService.ChangePassword(userData.AccountId, model);

				if (result)
				{
					TempData["success"] = "Password updated successfully!";
					return RedirectToAction("Profile");
				}
				else
				{
					TempData["error"] = "Current password is incorrect.";
				}
			}
			else
			{
				TempData["error"] = "Please check your input.";
			}

			return RedirectToAction("Profile");
		}
	}
}
