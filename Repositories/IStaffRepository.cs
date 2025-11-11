using System.Collections.Generic;
using BussinessObject;
using DTO;

namespace Repositories
{
    public interface IStaffRepository
    {
        List<Staff> GetAll();
        Staff? GetById(string staffId);
        StaffDTO? GetStaffWithRole(string staffId);
        void Add(Staff staff);
        void Update(Staff staff);
        void Delete(Staff staff);
    }
}
