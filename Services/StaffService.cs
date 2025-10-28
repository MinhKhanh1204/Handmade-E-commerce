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

        public void Add(StaffDTO staffDto)
        {
            var account = new Account
            {
                AccountId = staffDto.StaffId,
                Username = staffDto.Username ?? staffDto.StaffId,
                Email = staffDto.Email ?? "default@email.com",
                Password = staffDto.Password ?? "123456",
                CreatedAt = DateTime.Now,
                Status = "Active"
            };
            _context.Accounts.Add(account);

            var staff = new Staff
            {
                StaffId = staffDto.StaffId,
                FullName = staffDto.FullName,
                DateOfBirth = staffDto.DateOfBirth,
                Gender = staffDto.Gender,
                Phone = staffDto.Phone,
                CitizenId = staffDto.CitizenId,
                Address = staffDto.Address,
                HireDate = staffDto.HireDate,
                Status = staffDto.Status,
                StaffNavigation = account
            };
            _context.Staffs.Add(staff);

            _context.SaveChanges();
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

        public void Delete(string staffId)
        {
            var staff = _context.Staffs
                .Include(s => s.StaffNavigation)
                .FirstOrDefault(s => s.StaffId == staffId);

            if (staff == null) return;

            _context.Staffs.Remove(staff);

            if (staff.StaffNavigation != null)
                _context.Accounts.Remove(staff.StaffNavigation);

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
