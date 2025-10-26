using BussinessObject;
using HandicraftShop_Prodject.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace HandicraftShop_Prodject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VoucherController : Controller
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        // GET: Admin/Voucher
        public async Task<IActionResult> Index(VoucherSearchVm vm)
        {
            try
            {
                var result = await _voucherService.SearchAsync(
                    vm.Q, vm.IsActive, vm.ExpireFrom, vm.ExpireTo, 
                    vm.MinOrderFrom, vm.MinOrderTo, vm.Page, vm.PageSize, 
                    vm.SortBy, false);

                var pagedList = new PagedList<Voucher>
                {
                    Items = result.Items,
                    Total = result.Total,
                    Page = vm.Page,
                    PageSize = vm.PageSize
                };

                ViewBag.SearchVm = vm;
                return View(pagedList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading vouchers: {ex.Message}";
                return View(new PagedList<Voucher>());
            }
        }

        // GET: Admin/Voucher/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var voucher = await _voucherService.GetByIdAsync(id);
                if (voucher == null)
                {
                    TempData["ErrorMessage"] = "Voucher not found.";
                    return RedirectToAction(nameof(Index));
                }

                var vm = new VoucherDetailsVm
                {
                    VoucherId = voucher.VoucherId,
                    VoucherName = voucher.VoucherName!,
                    Code = voucher.Code!,
                    Description = voucher.Description,
                    DiscountPercentage = voucher.DiscountPercentage ?? 0,
                    MaxReducing = voucher.MaxReducing ?? 0,
                    Quantity = voucher.Quantity ?? 0,
                    UsageCount = voucher.UsageCount ?? 0,
                    ExpiryDate = voucher.ExpiryDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now,
                    IsActive = voucher.IsActive ?? false,
                    MinOrderValue = voucher.MinOrderValue ?? 0,
                    MaxUsagePerUser = voucher.MaxUsagePerUser ?? 0
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading voucher details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/Voucher/Test
        public async Task<IActionResult> Test()
        {
            try
            {
                // Test database connection and get all vouchers
                var allVouchers = await _voucherService.SearchAsync(null, null, null, null, null, null, 1, 100, null, false);
                
                var result = new
                {
                    TotalVouchers = allVouchers.Total,
                    VoucherCodes = allVouchers.Items.Select(v => v.Code).ToList(),
                    Message = "Database connection successful"
                };
                
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message, Message = "Database connection failed" });
            }
        }

        // GET: Admin/Voucher/TestCode?code=ABC
        public async Task<IActionResult> TestCode(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return Json(new { Error = "Code is required" });

                // Test database connection
                var testResult = await _voucherService.SearchAsync(null, null, null, null, null, null, 1, 10, null, false);
                
                // Test code check
                var isTaken = await _voucherService.IsCodeTakenAsync(code);
                
                return Json(new 
                { 
                    Code = code,
                    TotalVouchers = testResult.Total,
                    IsTaken = isTaken,
                    Message = "Test successful",
                    AllCodes = testResult.Items.Select(v => v.Code).ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new 
                { 
                    Error = ex.Message, 
                    StackTrace = ex.StackTrace,
                    Message = "Test failed" 
                });
            }
        }

        // GET: Admin/Voucher/IsCodeAvailable
        [AcceptVerbs("GET")]
        public async Task<IActionResult> IsCodeAvailable(string code)
        {
            try
            {
                Console.WriteLine($"=== IsCodeAvailable called with code: '{code}' ===");
                
                if (string.IsNullOrWhiteSpace(code))
                {
                    Console.WriteLine("Code is null or empty, returning true");
                    return Json(true);
                }

                // Simple test first
                var isTaken = await _voucherService.IsCodeTakenAsync(code);
                
                Console.WriteLine($"IsCodeAvailable result: Code='{code}', IsTaken={isTaken}");
                
                // Return false if taken (to show error), true if available
                return Json(isTaken ? "Mã voucher này đã tồn tại" : true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IsCodeAvailable ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(true); // Allow if error
            }
        }

        // GET: Admin/Voucher/Create
        public IActionResult Create()
        {
            var vm = new VoucherCreateVm(); // đã có default
            if (vm.ExpiryDate == DateTime.MinValue) 
                vm.ExpiryDate = DateTime.Today.AddDays(1);
            return View(vm);
        }

        // POST: Admin/Voucher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VoucherCreateVm vm)
        {
            try
            {
                // Server-side validation for expiry date
                if (vm.ExpiryDate.Date <= DateTime.Today)
                    ModelState.AddModelError(nameof(vm.ExpiryDate), "Ngày hết hạn phải lớn hơn ngày hiện tại.");

                if (!ModelState.IsValid)
                {
                    return View(vm);
                }

                // Double-check if code is taken (defense against race conditions)
                if (await _voucherService.IsCodeTakenAsync(vm.Code!))
                {
                    ModelState.AddModelError(nameof(vm.Code), "Mã voucher này đã tồn tại.");
                    return View(vm);
                }

                var voucher = new Voucher
                {
                    VoucherName = vm.VoucherName,
                    Code = vm.Code,
                    Description = vm.Description,
                    DiscountPercentage = vm.DiscountPercentage,
                    MaxReducing = vm.MaxReducing,
                    Quantity = vm.Quantity,
                    UsageCount = 0,
                    ExpiryDate = DateOnly.FromDateTime(vm.ExpiryDate),
                    IsActive = vm.IsActive,
                    MinOrderValue = vm.MinOrderValue,
                    MaxUsagePerUser = vm.MaxUsagePerUser
                };

                await _voucherService.CreateAsync(voucher);
                TempData["SuccessMessage"] = "Tạo voucher thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true || ex.InnerException?.Message.Contains("duplicate") == true)
            {
                // Handle unique constraint violation (race condition)
                ModelState.AddModelError(nameof(vm.Code), "Mã voucher này đã tồn tại.");
                return View(vm);
            }
            catch (InvalidOperationException ex)
            {
                // Handle business logic validation errors
                ModelState.AddModelError(nameof(vm.Code), ex.Message);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tạo voucher: {ex.Message}";
                return View(vm);
            }
        }

        // GET: Admin/Voucher/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var voucher = await _voucherService.GetByIdAsync(id);
                if (voucher == null)
                {
                    TempData["ErrorMessage"] = "Voucher not found.";
                    return RedirectToAction(nameof(Index));
                }

                var vm = new VoucherEditVm
                {
                    VoucherId = voucher.VoucherId,
                    VoucherName = voucher.VoucherName!,
                    Code = voucher.Code!,
                    Description = voucher.Description,
                    DiscountPercentage = voucher.DiscountPercentage ?? 0,
                    MaxReducing = voucher.MaxReducing ?? 0,
                    Quantity = voucher.Quantity ?? 0,
                    UsageCount = voucher.UsageCount ?? 0,
                    ExpiryDate = voucher.ExpiryDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now,
                    IsActive = voucher.IsActive ?? false,
                    MinOrderValue = voucher.MinOrderValue ?? 0,
                    MaxUsagePerUser = voucher.MaxUsagePerUser ?? 0
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading voucher for edit: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/Voucher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VoucherEditVm vm)
        {
            try
            {
                if (id != vm.VoucherId)
                {
                    TempData["ErrorMessage"] = "Invalid voucher ID.";
                    return RedirectToAction(nameof(Index));
                }

                // Server-side validation for expiry date
                if (vm.ExpiryDate.Date <= DateTime.Today)
                    ModelState.AddModelError(nameof(vm.ExpiryDate), "Ngày hết hạn phải lớn hơn ngày hiện tại.");

                if (!ModelState.IsValid)
                {
                    return View(vm);
                }

                // Check if code is unique (excluding current voucher)
                if (!await _voucherService.IsCodeUniqueAsync(vm.Code!, vm.VoucherId))
                {
                    ModelState.AddModelError(nameof(vm.Code), "Mã voucher này đã tồn tại.");
                    return View(vm);
                }

                var voucher = await _voucherService.GetByIdAsync(id);
                if (voucher == null)
                {
                    TempData["ErrorMessage"] = "Voucher not found.";
                    return RedirectToAction(nameof(Index));
                }

                voucher.VoucherName = vm.VoucherName;
                voucher.Code = vm.Code;
                voucher.Description = vm.Description;
                voucher.DiscountPercentage = vm.DiscountPercentage;
                voucher.MaxReducing = vm.MaxReducing;
                voucher.Quantity = vm.Quantity;
                voucher.ExpiryDate = DateOnly.FromDateTime(vm.ExpiryDate);
                voucher.IsActive = vm.IsActive;
                voucher.MinOrderValue = vm.MinOrderValue;
                voucher.MaxUsagePerUser = vm.MaxUsagePerUser;

                await _voucherService.UpdateAsync(voucher);
                TempData["SuccessMessage"] = "Cập nhật voucher thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating voucher: {ex.Message}";
                return View(vm);
            }
        }

        // GET: Admin/Voucher/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var voucher = await _voucherService.GetByIdAsync(id);
                if (voucher == null)
                {
                    TempData["ErrorMessage"] = "Voucher not found.";
                    return RedirectToAction(nameof(Index));
                }

                var vm = new VoucherDeleteVm
                {
                    VoucherId = voucher.VoucherId,
                    VoucherName = voucher.VoucherName!,
                    Code = voucher.Code!,
                    DiscountPercentage = voucher.DiscountPercentage ?? 0,
                    ExpiryDate = voucher.ExpiryDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now,
                    IsActive = voucher.IsActive ?? false
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading voucher for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/Voucher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _voucherService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Voucher deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting voucher: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
