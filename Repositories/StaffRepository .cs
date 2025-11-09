using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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
