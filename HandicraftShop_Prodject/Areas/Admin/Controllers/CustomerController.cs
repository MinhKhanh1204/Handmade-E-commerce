using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using BussinessObject;
using System.Linq;

namespace HandicraftShop_Prodject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // UC_43: View customers with search
        public async Task<IActionResult> Index(string searchString)
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"Search called with: '{searchString}'");
            
            IEnumerable<Customer> customers;
            
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                customers = await _customerService.SearchCustomersAsync(searchString);
                ViewData["CurrentFilter"] = searchString;
                System.Diagnostics.Debug.WriteLine($"Search found {customers.Count()} customers");
            }
            else
            {
                customers = await _customerService.GetAllCustomersAsync();
                System.Diagnostics.Debug.WriteLine($"No search string, showing all {customers.Count()} customers");
            }
            
            // Check if this is an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_CustomerTable", customers);
            }
            
            return View(customers);
        }

        // UC_46: View customer details
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var customer = await _customerService.GetCustomerDetailsAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // UC_45: Edit customer account - GET
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // UC_45: Edit customer account - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Customer customer)
        {
            System.Diagnostics.Debug.WriteLine($"Edit POST called with id: '{id}', customer.CustomerId: '{customer?.CustomerId}'");
            
            if (id != customer?.CustomerId)
            {
                System.Diagnostics.Debug.WriteLine($"ID mismatch: id='{id}', customer.CustomerId='{customer?.CustomerId}'");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("ModelState is invalid:");
                foreach (var error in ModelState)
                {
                    System.Diagnostics.Debug.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return View(customer);
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("Calling UpdateCustomerAsync...");
                await _customerService.UpdateCustomerAsync(customer);
                TempData["SuccessMessage"] = "Customer updated successfully!";
                System.Diagnostics.Debug.WriteLine("Customer updated successfully, redirecting to Index");
                return Redirect("/Admin/Customer");
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine($"ArgumentException: {ex.Message}");
                ModelState.AddModelError("", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"InvalidOperationException: {ex.Message}");
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Exception: {ex.Message}");
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View(customer);
        }

        // Delete customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Customer deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Customer not found!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting customer: {ex.Message}";
            }

            return Redirect("/Admin/Customer");
        }
    }
}
