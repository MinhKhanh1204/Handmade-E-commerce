using System.IO;
using System.Linq;
using BussinessObject;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;

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

        #region Index
        public IActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var staffs = _staffService.GetAll().AsQueryable();

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

            var pagedStaffs = staffs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = searchString;

            return View(pagedStaffs);
        }
        #endregion

        #region Create
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

            // Upload avatar nếu có
            if (staffDto.Avatar != null && staffDto.Avatar.Length > 0)
            {
                staffDto.AvatarUrl = SaveAvatar(staffDto.Avatar);
            }

            if (!string.IsNullOrEmpty(staffDto.Password))
            {
                staffDto.Password = HashPassword(staffDto.Password);
            }

            _staffService.Add(staffDto);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit
        // GET: Admin/Staffs/Edit/{id}
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var staff = _staffService.GetByIdEntity(id);
            if (staff == null)
                return NotFound();

            var staffDto = new StaffDTO
            {
                StaffId = staff.StaffId,
                FullName = staff.FullName,
                Gender = staff.Gender,
                Phone = staff.Phone,
                Address = staff.Address,
                Status = staff.Status,
                DateOfBirth = staff.DateOfBirth,
                HireDate = staff.HireDate,
                CitizenId = staff.CitizenId,
                AvatarUrl = staff.StaffNavigation?.Avatar,
                Email = staff.StaffNavigation?.Email,
                Username = staff.StaffNavigation?.Username
            };

            // Chuẩn bị SelectList cho dropdown
            ViewBag.Genders = new List<SelectListItem>
    {
        new SelectListItem { Text = "Male", Value = "Male" },
        new SelectListItem { Text = "Female", Value = "Female" }
    };

            ViewBag.Statuses = new List<SelectListItem>
    {
        new SelectListItem { Text = "Active", Value = "Active" },
        new SelectListItem { Text = "Inactive", Value = "Inactive" }
    };

            return View(staffDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StaffDTO staffDto)
        {
            if (!ModelState.IsValid)
                return View(staffDto);

            var staff = _staffService.GetByIdEntity(staffDto.StaffId);
            if (staff == null)
                return NotFound();

            // Cập nhật thông tin Staff
            staff.FullName = staffDto.FullName;
            staff.Gender = staffDto.Gender;
            staff.Phone = staffDto.Phone;
            staff.Address = staffDto.Address;
            staff.Status = staffDto.Status;

            if (staff.StaffNavigation != null)
            {
                staff.StaffNavigation.Username = staffDto.Username ?? staff.StaffNavigation.Username;
                staff.StaffNavigation.Email = staffDto.Email ?? staff.StaffNavigation.Email;

                if (staffDto.Avatar != null && staffDto.Avatar.Length > 0)
                    staff.StaffNavigation.Avatar = SaveAvatar(staffDto.Avatar);
            }

            _staffService.UpdateEntity(staff);
            return RedirectToAction(nameof(Index));
        }


        #endregion

        #region Details
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var staff = _staffService.GetByIdEntity(id);
            if (staff == null)
                return NotFound();

            var staffDto = new StaffDTO
            {
                StaffId = staff.StaffId,
                FullName = staff.FullName,
                Gender = staff.Gender,
                Phone = staff.Phone,
                Address = staff.Address,
                Status = staff.Status,
                DateOfBirth = staff.DateOfBirth,
                HireDate = staff.HireDate,
                CitizenId = staff.CitizenId,
                AvatarUrl = staff.StaffNavigation?.Avatar,
                Email = staff.StaffNavigation?.Email,
                Username = staff.StaffNavigation?.Username
            };

            return View(staffDto);
        }
        #endregion

        #region Delete
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
        #endregion

        #region Helpers
        private string SaveAvatar(IFormFile avatar)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
            var path = Path.Combine("wwwroot/images/avatars", fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                avatar.CopyTo(stream);
            }
            return $"/images/avatars/{fileName}";
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
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
        #endregion
    }
}
