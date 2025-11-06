using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Statistics
{
    public class LoyalCustomerDTO
    {
        public string CustomerId { get; set; } = null!;
        public string? CustomerName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public string? Phone { get; set; }
    }
}
