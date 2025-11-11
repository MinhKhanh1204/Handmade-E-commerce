using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BussinessObject;
using DataAccessObject;
using DTO;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class StaffService : IStaffService
    {
        private readonly MyStoreContext _context;

        public StaffService(MyStoreContext context)
        {
            _context = context;
        }

        public List<StaffDTO> GetAll()
        {
            return _context.Staffs
                .Include(s => s.StaffNavigation)
                .Select(s => new StaffDTO
                {
                    StaffId = s.StaffId,
                    FullName = s.FullName,
                    DateOfBirth = s.DateOfBirth,
                    Gender = s.Gender,
                    Phone = s.Phone,
                    CitizenId = s.CitizenId,
                    Address = s.Address,
                    HireDate = s.HireDate,
                    Status = s.Status,
                    Username = s.StaffNavigation.Username,
                    Email = s.StaffNavigation.Email
                })
                .ToList();
        }

        public StaffDTO? GetById(string staffId)
        {
            var staff = _context.Staffs
                .Include(staff => staff.StaffNavigation)
                .FirstOrDefault(x => x.StaffId == staffId);

            if (staff == null) return null;

            var roles = staff.StaffNavigation?.UserRoles
        .Where(ur => ur.Status == "Active")
        .Select(ur => ur.Role.RoleName)
        .ToList();

            return new StaffDTO
            {
                StaffId = staff.StaffId,
                FullName = staff.FullName,
                Gender = staff.Gender,
                Phone = staff.Phone,
                Address = staff.Address,
                AvatarUrl = staff.StaffNavigation?.Avatar,
                Email = staff.StaffNavigation?.Email,
                Username = staff.StaffNavigation?.Username,
                Roles = roles // Thêm property Roles: List<string>
            };
        }

        public void Add(StaffDTO staffDto)
        {
            var requestedId = staffDto.StaffId;
            if (string.IsNullOrWhiteSpace(requestedId) ||
                _context.Staffs.Any(s => s.StaffId == requestedId) ||
                _context.Accounts.Any(a => a.AccountId == requestedId))
            {
                requestedId = GenerateNewStaffId();
            }

            // Thêm: Gán avatar path nếu có
            string avatarUrl = staffDto.AvatarUrl ?? "/images/avatars/default.png"; // fallback ảnh mặc định

            var account = new Account
            {
                AccountId = requestedId,
                Username = staffDto.Username ?? requestedId,
                Email = staffDto.Email ?? "default@email.com",
                Password = staffDto.Password ?? "123456",
                CreatedAt = DateTime.Now,
                Status = "Active",
                Avatar = avatarUrl // ✅ Lưu avatar vào cột Avatar của bảng Accounts
            };
            _context.Accounts.Add(account);

            var staff = new Staff
            {
                StaffId = requestedId,
                FullName = staffDto.FullName,
                DateOfBirth = staffDto.DateOfBirth,
                Gender = staffDto.Gender,
                Phone = staffDto.Phone,
                CitizenId = staffDto.CitizenId,
                Address = staffDto.Address,
                HireDate = staffDto.HireDate,
                Status = staffDto.Status ?? "Active",
                StaffNavigation = account
            };
            _context.Staffs.Add(staff);

            _context.SaveChanges();
            staffDto.StaffId = requestedId;
        }

        private string GenerateNewStaffId()
        {
            var staffIds = _context.Staffs.Select(s => s.StaffId)
                              .Concat(_context.Accounts.Select(a => a.AccountId))
                              .Where(id => id != null && id.StartsWith("STF"))
                              .AsEnumerable()
                              .ToList();

            int max = 0;
            foreach (var id in staffIds)
            {
                if (id.Length > 3)
                {
                    var numPart = id.Substring(3);
                    if (int.TryParse(numPart, out int n) && n > max)
                        max = n;
                }
            }

            return $"STF{(max + 1):D3}";
        }

        public void Update(StaffDTO staffDto)
        {
            var staff = _context.Staffs
                .Include(s => s.StaffNavigation)
                .FirstOrDefault(s => s.StaffId == staffDto.StaffId);

            if (staff == null) return;

            staff.FullName = staffDto.FullName;
            staff.Gender = staffDto.Gender;
            staff.Phone = staffDto.Phone;
            staff.Address = staffDto.Address;
            staff.Status = staffDto.Status;

            if (staff.StaffNavigation != null)
            {
                staff.StaffNavigation.Username = staffDto.Username ?? staff.StaffNavigation.Username;
                staff.StaffNavigation.Email = staffDto.Email ?? staff.StaffNavigation.Email;
            }

            _context.SaveChanges();
        }

        public void Delete(string staffId)
        {
            var staff = _context.Staffs
                .Include(s => s.StaffNavigation)
                .FirstOrDefault(s => s.StaffId == staffId);

            if (staff == null) return;

            staff.Status = "Deleted";
            if (staff.StaffNavigation != null)
                staff.StaffNavigation.Status = "Deleted";

            _context.Staffs.Update(staff);
            _context.SaveChanges();
        }

        public Staff GetByIdEntity(string staffId)
        {
            return _context.Staffs
                .Include(s => s.StaffNavigation)
                .FirstOrDefault(s => s.StaffId == staffId)!;
        }

        public void UpdateEntity(Staff staff)
        {
            _context.Staffs.Update(staff);
            _context.SaveChanges();
        }
    }
}
