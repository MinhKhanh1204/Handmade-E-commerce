using BussinessObject;
using DTO;
using System.Collections.Generic;

namespace Services
{
    public interface IStaffService
    {
        List<StaffDTO> GetAll();
        StaffDTO? GetById(string staffId);
        void Add(StaffDTO staffDto);
        void Update(StaffDTO staffDto);
        void Delete(string staffId);

        // Thêm 2 dòng này để dùng trong Controller
        Staff GetByIdEntity(string staffId);
        void UpdateEntity(Staff staff);
    }
}
