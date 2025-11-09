using BussinessObject;
using System.Collections.Generic;

namespace Repositories
{
    public interface IStaffRepository
    {
        List<Staff> GetAll();
        Staff? GetById(string staffId);
        void Add(Staff staff);
        void Update(Staff staff);
        void Delete(Staff staff);
    }
}
