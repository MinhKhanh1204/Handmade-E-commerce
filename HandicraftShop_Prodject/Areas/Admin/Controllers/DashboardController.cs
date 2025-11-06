using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;
using DTO.Statistics;

namespace HandicraftShop_Prodject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IStatisticService _statService;

        public DashboardController(IStatisticService statService)
        {
            _statService = statService;
        }

        public async Task<IActionResult> Index(int? year, DateTime? from = null, DateTime? to = null)
        {
            int filterYear = year ?? DateTime.Now.Year;
            var vm = new DashboardStatisticDTO
            {
                From = from,
                To = to,
                TotalRevenue = await _statService.GetTotalRevenueAsync(from, to),
                TotalOrders = await _statService.GetTotalOrdersAsync(from, to),
                Feedbacks = await _statService.GetFeedbackStatisticAsync(),
                TopEmployees = await _statService.GetTopEmployeesAsync(from, to),
                LoyalCustomers = await _statService.GetLoyalCustomersAsync(),
                MonthlyData = await _statService.GetMonthlyRevenueAndOrdersAsync(DateTime.Now.Year),
                YearlyData = await _statService.GetYearlyRevenueAndOrdersAsync()
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyData(int year)
        {
            var data = await _statService.GetMonthlyRevenueAndOrdersAsync(year);
            return Json(data);
        }
    }
}
