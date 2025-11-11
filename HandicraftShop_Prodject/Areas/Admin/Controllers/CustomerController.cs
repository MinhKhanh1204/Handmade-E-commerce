using Microsoft.AspNetCore.Mvc;
using Services;
using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace HandicraftShop_Prodject.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CustomerController : Controller
	{
		private readonly ICustomerService _customerService;
		private readonly MyStoreContext _context;
		private readonly IWebHostEnvironment _env;

		public CustomerController(ICustomerService customerService, MyStoreContext context, IWebHostEnvironment env)
		{
			_customerService = customerService;
			_context = context;
			_env = env;
		}

		public async Task<IActionResult> Index(string searchString)
		{
			IEnumerable<Customer> customers;
			if (!string.IsNullOrWhiteSpace(searchString))
			{
				customers = await _customerService.SearchCustomersAsync(searchString);
				ViewData["CurrentFilter"] = searchString;
			}
			else
			{
				customers = await _customerService.GetAllCustomersAsync();
			}

			return View(customers);
		}

		public async Task<IActionResult> Details(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();
			var customer = await _customerService.GetCustomerDetailsAsync(id);
			if (customer == null) return NotFound();
			return View(customer);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();
			var customer = await _customerService.GetCustomerByIdAsync(id);
			if (customer == null) return NotFound();
			return View(customer);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Customer customer, IFormFile? Avatar)
		{
			// Ignore validation for navigation properties that are not part of the form post
			ModelState.Remove(nameof(Customer.CustomerNavigation));

			if (!ModelState.IsValid)
			{
				return View(customer);
			}

			await _customerService.UpdateCustomerAsync(customer);

			// Handle avatar upload to linked Account (stored at Accounts.Avatar)
			if (Avatar != null && Avatar.Length > 0)
			{
				// Load customer with navigation to ensure correct principal is updated
				var customerEntity = await _context.Customers
					.Include(c => c.CustomerNavigation)
					.FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId);

				if (customerEntity?.CustomerNavigation != null)
				{
					// Save under wwwroot so static file middleware can serve it
					var uploadsDir = Path.Combine(_env.WebRootPath, "assets", "img", "person");
					if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

					// Generate unique file name to avoid browser cache and name collisions
					var ext = Path.GetExtension(Avatar.FileName);
					var safeExt = string.IsNullOrWhiteSpace(ext) ? ".jpg" : ext.ToLowerInvariant();
					var uniqueFileName = $"{customer.CustomerId}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{safeExt}";
					var filePath = Path.Combine(uploadsDir, uniqueFileName);

					// Delete old avatar file if exists (and not default)
					var currentAvatar = customerEntity.CustomerNavigation.Avatar;
					if (!string.IsNullOrWhiteSpace(currentAvatar))
					{
						var oldPath = Path.Combine(uploadsDir, currentAvatar);
						if (System.IO.File.Exists(oldPath))
						{
							try { System.IO.File.Delete(oldPath); } catch { /* ignore */ }
						}
					}

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await Avatar.CopyToAsync(stream);
					}

					customerEntity.CustomerNavigation.Avatar = uniqueFileName;
					await _context.SaveChangesAsync();
				}
				else
				{
					// If no Account linked, notify user
					TempData["ErrorMessage"] = "Customer updated, but no linked account found to save avatar.";
				}
			}

			TempData["SuccessMessage"] = "Customer updated successfully!";
			return Redirect("/Admin/Customer");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return Redirect("/Admin/Customer");
			var ok = await _customerService.DeleteCustomerAsync(id);
			TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Customer deleted successfully!" : "Customer not found!";
			return Redirect("/Admin/Customer");
		}
	}
}


