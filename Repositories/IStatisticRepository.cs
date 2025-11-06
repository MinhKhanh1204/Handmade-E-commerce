using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Statistics;

namespace Repositories
{
    public interface IStatisticRepository
    {
        Task<decimal> GetTotalRevenueAsync(DateTime? from = null, DateTime? to = null);
        Task<int> GetTotalOrdersAsync(DateTime? from = null, DateTime? to = null);
        Task<List<EmployeeSalesDTO>> GetTopEmployeesAsync(DateTime? from = null, DateTime? to = null);
        Task<List<FeedbackStatisticDTO>> GetFeedbackStatisticAsync();
        Task<List<LoyalCustomerDTO>> GetLoyalCustomersAsync();
        Task<List<MonthlyStatistic>> GetMonthlyRevenueAndOrdersAsync(int year);
        Task<List<YearlyStatistic>> GetYearlyRevenueAndOrdersAsync();


    }
}