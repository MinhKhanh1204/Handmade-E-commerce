using System.Collections.Generic;
using System.Linq;
using BussinessObject;
using DataAccessObject;
using DTO;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly MyStoreContext _context;

        public StaffRepository(MyStoreContext context)
        {
            _context = context;
        }

        public List<Staff> GetAll()
        {
            return _context.Staffs
                .Include(s => s.StaffNavigation)
                .ToList();
        }

        public Staff? GetById(string staffId)
        {
            return _context.Staffs
                .Include(s => s.StaffNavigation)
                .FirstOrDefault(s => s.StaffId == staffId);
        }

        public StaffDTO? GetStaffWithRole(string staffId)
        {
            var staff = _context.Staffs
                .Include(s => s.StaffNavigation)
                    .ThenInclude(a => a.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .FirstOrDefault(s => s.StaffId == staffId);

            if (staff == null) return null;

            // Lấy tất cả role active
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
                Roles = roles,
                RoleName = roles?.FirstOrDefault() // nếu muốn chỉ lấy 1 role
            };
        }

        public void Add(Staff staff)
        {
            _context.Staffs.Add(staff);
            _context.SaveChanges();
        }

        public void Update(Staff staff)
        {
            _context.Staffs.Update(staff);
            _context.SaveChanges();
        }

        // Soft-delete: set status to "Inactive" instead of removing DB row
        public void Delete(Staff staff)
        {
            staff.Status = "Inactive";
            if (staff.StaffNavigation != null)
                staff.StaffNavigation.Status = "Inactive";

            _context.Staffs.Update(staff);
            _context.SaveChanges();
        }
    }
}
