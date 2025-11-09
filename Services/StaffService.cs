using System;
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

        // Return all staffs (including Inactive) so admin Index shows deleted/inactive staffs.
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
            var s = _context.Staffs
                .Include(s => s.StaffNavigation)
                .FirstOrDefault(x => x.StaffId == staffId);

            if (s == null) return null;

            return new StaffDTO
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
            };
        }

        // Add staff: ensure unique StaffId (avoid PK violation)
        public void Add(StaffDTO staffDto)
        {
            // If no id provided or id collides, generate a unique one
            var requestedId = staffDto.StaffId;
            if (string.IsNullOrWhiteSpace(requestedId) ||
                _context.Staffs.Any(s => s.StaffId == requestedId) ||
                _context.Accounts.Any(a => a.AccountId == requestedId))
            {
                requestedId = GenerateNewStaffId();
            }

            // Prepare account and staff entities
            var account = new Account
            {
                AccountId = requestedId,
                Username = staffDto.Username ?? requestedId,
                Email = staffDto.Email ?? "default@email.com",
                Password = staffDto.Password ?? "123456",
                CreatedAt = DateTime.Now,
                Status = "Active"
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

            // update caller DTO id (useful if controller shows created id)
            staffDto.StaffId = requestedId;
        }

        // Helper: generate next available STFxxx id using DB values (Staffs and Accounts)
        private string GenerateNewStaffId()
        {
            // Collect existing ids that look like STF###
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

            return $"STF{(max + 1).ToString("D3")}";
        }

        public void Update(StaffDTO staffDto)
        {
            var staff = _context.Staffs
                .Include(s => s.StaffNavigation)
                .FirstOrDefault(s => s.StaffId == staffDto.StaffId);

            if (staff == null) return;

            // Chỉ update các trường cho phép
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

        // Soft-delete staff & account by marking Status = "Deleted"
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

        // Entity trực tiếp (dùng cho Edit giữ nguyên các trường readonly)
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
