using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Statistics
{
    public class DashboardStatisticDTO
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<EmployeeSalesDTO> TopEmployees { get; set; } = new();
        public List<FeedbackStatisticDTO> Feedbacks { get; set; } = new();
        public List<LoyalCustomerDTO> LoyalCustomers { get; set; } = new();
        public string Type { get; set; }
        public int Year { get; set; }
        public List<MonthlyStatistic>? MonthlyData { get; set; }
        public List<YearlyStatistic>? YearlyData { get; set; }
    }

}
