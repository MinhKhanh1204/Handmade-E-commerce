using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BussinessObject;
using DataAccessObject;
using DTO.Statistics;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly MyStoreContext _context;

        public StatisticRepository(MyStoreContext context) => _context = context;

        // UC_77 - View total revenue statistics
        public async Task<decimal> GetTotalRevenueAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Orders
                .Where(o => o.PaymentStatus == "Paid");

            if (from.HasValue)
                query = query.Where(o => o.OrderDate >= from);
            if (to.HasValue)
                query = query.Where(o => o.OrderDate <= to);

            // ✅ xử lý nullable TotalAmount
            return await query.SumAsync(o => o.TotalAmount ?? 0);
        }

        // UC_78 - View total orders sold statistics
        public async Task<int> GetTotalOrdersAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Orders.AsQueryable();

            if (from.HasValue)
                query = query.Where(o => o.OrderDate >= from);
            if (to.HasValue)
                query = query.Where(o => o.OrderDate <= to);

            return await query.CountAsync();
        }

        // UC_79 - View top 10 employee sales performance
        public async Task<List<EmployeeSalesDTO>> GetTopEmployeesAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Orders
                .Include(o => o.Staff)
                .Where(o => o.PaymentStatus == "Paid");

            if (from.HasValue)
                query = query.Where(o => o.OrderDate >= from);
            if (to.HasValue)
                query = query.Where(o => o.OrderDate <= to);

            var result = await query
                .GroupBy(o => o.Staff.FullName)
                .Select(g => new EmployeeSalesDTO
                {
                    EmployeeName = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalAmount ?? 0),
                    OrdersCount = g.Count()
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(10)
                .ToListAsync();

            return result;
        }

        // UC_81 - Positive and negative feedback statistic
        public async Task<List<FeedbackStatisticDTO>> GetFeedbackStatisticAsync()
        {
            var result = await _context.Feedbacks
                .GroupBy(f => f.Rating >= 4 ? "Positive" : "Negative")
                .Select(g => new FeedbackStatisticDTO
                {
                    Type = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return result;
        }

        // UC_82 - Loyal customer statistic
        public async Task<List<LoyalCustomerDTO>> GetLoyalCustomersAsync()
        {
            var result = await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.PaymentStatus == "Paid")
                .GroupBy(o => o.Customer.FullName)
                .Select(g => new LoyalCustomerDTO
                {
                    CustomerName = g.Key,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(x => x.TotalAmount ?? 0)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(10)
                .ToListAsync();

            return result;
        }

        public async Task<List<MonthlyStatistic>> GetMonthlyRevenueAndOrdersAsync(int year)
        {
            var query = _context.Orders
                .Where(o => o.PaymentStatus == "Paid" && o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
                .GroupBy(o => o.OrderDate.Value.Month)
                .Select(g => new MonthlyStatistic
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalAmount ?? 0),
                    TotalOrders = g.Count()
                })
                .OrderBy(g => g.Month);

            return await query.ToListAsync();
        }

        public async Task<List<YearlyStatistic>> GetYearlyRevenueAndOrdersAsync()
        {
            var query = _context.Orders
                .Where(o => o.PaymentStatus == "Paid" && o.OrderDate.HasValue)
                .GroupBy(o => o.OrderDate.Value.Year)
                .Select(g => new YearlyStatistic
                {
                    Year = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalAmount ?? 0),
                    TotalOrders = g.Count()
                })
                .OrderBy(g => g.Year);

            return await query.ToListAsync();
        }

    }
}
