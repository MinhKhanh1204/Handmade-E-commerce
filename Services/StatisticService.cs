using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTO;
using DTO.Statistics;
using Repositories;

namespace Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository _repo;

        public StatisticService(IStatisticRepository repo) => _repo = repo;

        public async Task<DashboardStatisticDTO> GetDashboardAsync(DateTime? from = null, DateTime? to = null)
        {
            var totalRevenue = await _repo.GetTotalRevenueAsync(from, to);
            var totalOrders = await _repo.GetTotalOrdersAsync(from, to);
            var topEmployees = await _repo.GetTopEmployeesAsync(from, to);

            return new DashboardStatisticDTO
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TopEmployees = topEmployees
            };
        }

        public Task<List<EmployeeSalesDTO>> GetTopEmployeesAsync(DateTime? from = null, DateTime? to = null)
            => _repo.GetTopEmployeesAsync(from, to);

        public Task<List<FeedbackStatisticDTO>> GetFeedbackStatisticAsync()
            => _repo.GetFeedbackStatisticAsync();

        public Task<List<LoyalCustomerDTO>> GetLoyalCustomersAsync()
            => _repo.GetLoyalCustomersAsync();

        public Task<List<MonthlyStatistic>> GetMonthlyRevenueAndOrdersAsync(int year)
            => _repo.GetMonthlyRevenueAndOrdersAsync(year);

        public Task<List<YearlyStatistic>> GetYearlyRevenueAndOrdersAsync()
            => _repo.GetYearlyRevenueAndOrdersAsync();

        public Task<decimal> GetTotalRevenueAsync(DateTime? from = null, DateTime? to = null)
            => _repo.GetTotalRevenueAsync(from, to);

        public Task<int> GetTotalOrdersAsync(DateTime? from = null, DateTime? to = null)
            => _repo.GetTotalOrdersAsync(from, to);
    }
}
