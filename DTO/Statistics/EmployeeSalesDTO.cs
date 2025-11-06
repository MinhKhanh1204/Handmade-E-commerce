using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Statistics
{
    public class EmployeeSalesDTO
    {
        public string StaffId { get; set; } = null!;
        public string? EmployeeName { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
