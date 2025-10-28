using DTO;
using Microsoft.AspNetCore.Mvc;
using Services;
using BussinessObject;
using System.Linq;

namespace HandicraftShop_Prodject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StaffsController : Controller
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        public IActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var staffs = _staffService.GetAll().AsQueryable();

            // Search theo StaffId, FullName hoặc Phone
            if (!string.IsNullOrEmpty(searchString))
            {
                staffs = staffs.Where(s =>
                    s.StaffId.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (s.FullName != null && s.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (s.Phone != null && s.Phone.Contains(searchString))
                );
            }

            int totalRecords = staffs.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Lấy dữ liệu phân trang
            var pagedStaffs = staffs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Truyền dữ liệu pagination & search về view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = searchString;

            return View(pagedStaffs);
        }

        public IActionResult Create()
        {
            var staffDto = new StaffDTO
            {
                StaffId = GenerateStaffId()
            };
            return View(staffDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StaffDTO staffDto)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(staffDto.StaffId))
                    staffDto.StaffId = GenerateStaffId();
                return View(staffDto);
            }

            _staffService.Add(staffDto);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var staffDto = _staffService.GetById(id);
            if (staffDto == null)
                return NotFound();

            return View(staffDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StaffDTO staffDto)
        {
            if (!ModelState.IsValid)
                return View(staffDto);

            // Lấy entity từ DB để giữ các trường không edit
            var staff = _staffService.GetByIdEntity(staffDto.StaffId);
            if (staff == null)
                return NotFound();

            // Cập nhật các trường cho phép
            staff.FullName = staffDto.FullName;
            staff.Gender = staffDto.Gender;
            staff.Phone = staffDto.Phone;
            staff.Address = staffDto.Address;
            staff.Status = staffDto.Status;

            _staffService.UpdateEntity(staff);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var staffDto = _staffService.GetById(id);
            if (staffDto == null)
                return NotFound();

            return View(staffDto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _staffService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var staffDto = _staffService.GetById(id);
            if (staffDto == null)
                return NotFound();

            return View(staffDto);
        }

        private string GenerateStaffId()
        {
            var allStaffs = _staffService.GetAll();
            if (!allStaffs.Any())
                return "STF001";

            var lastId = allStaffs
                .Select(s => s.StaffId)
                .Where(s => s.StartsWith("STF"))
                .OrderByDescending(s => s)
                .FirstOrDefault();

            int lastNumber = 0;
            if (!string.IsNullOrEmpty(lastId))
                int.TryParse(lastId.Substring(3), out lastNumber);

            return $"STF{(lastNumber + 1).ToString("D3")}";
        }
    }
}
