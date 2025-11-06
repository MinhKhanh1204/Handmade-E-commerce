using DataAccessObject;
using BussinessObject;
using HandicraftShop_Prodject.Areas.Admin.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;

namespace HandicraftShop_Prodject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // ✅ UC_59: View all orders
        // 🔹 Default: hiển thị tab Pending khi load trang
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersForStaffAsync();
            return View(orders);
        }

        // ✅ UC_60: Search orders
        [HttpGet]
        public async Task<IActionResult> Search(string? orderId, string? customerName, DateTime? from, DateTime? to)
        {
            try
            {
                // Lấy toàn bộ đơn hàng
                var orders = await _orderService.GetAllOrdersForStaffAsync();

                // Lọc theo orderId
                if (!string.IsNullOrEmpty(orderId))
                    orders = orders.Where(o => o.OrderId.Contains(orderId, StringComparison.OrdinalIgnoreCase));

                // Lọc theo tên khách hàng
                if (!string.IsNullOrEmpty(customerName))
                    orders = orders.Where(o => o.Customer != null &&
                        o.Customer.FullName.Contains(customerName, StringComparison.OrdinalIgnoreCase));

                // Lọc theo ngày
                if (from.HasValue)
                    orders = orders.Where(o => o.OrderDate >= from.Value);
                if (to.HasValue)
                    orders = orders.Where(o => o.OrderDate <= to.Value);

                // Chia tab
                var pendingOrders = orders.Where(o => o.ShippingStatus == "Pending").ToList();
                var historyOrders = orders.Where(o => o.ShippingStatus != "Pending").ToList();

                // Render partial ra HTML
                var pendingHtml = await this.RenderViewAsync("_OrderListPartial", pendingOrders, true);
                var historyHtml = await this.RenderViewAsync("_OrderListPartial", historyOrders, true);

                // ✅ Trả kết quả đúng format cho JS
                return Json(new
                {
                    success = true,
                    pendingHtml,
                    historyHtml
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Sort(string sortBy, string direction = "asc")
        {
            var orders = await _orderService.GetAllOrdersForStaffAsync();

            orders = sortBy switch
            {
                "date" => direction == "desc"
                    ? orders.OrderByDescending(o => o.OrderDate)
                    : orders.OrderBy(o => o.OrderDate),
                "total" => direction == "desc"
                    ? orders.OrderByDescending(o => o.TotalAmount)
                    : orders.OrderBy(o => o.TotalAmount),
                "status" => direction == "desc"
                    ? orders.OrderByDescending(o => o.ShippingStatus)
                    : orders.OrderBy(o => o.ShippingStatus),
                _ => orders.OrderBy(o => o.OrderId)
            };

            var pendingHtml = await this.RenderViewAsync("_OrderListPartial",
                orders.Where(o => o.ShippingStatus == "Pending").ToList(), true);

            var historyHtml = await this.RenderViewAsync("_OrderListPartial",
                orders.Where(o => o.ShippingStatus != "Pending").ToList(), true);

            return Json(new
            {
                success = true,   // ✅ Thêm dòng này
                pendingHtml,
                historyHtml
            });
        }

        // ✅ UC_63: View order detail
        public async Task<IActionResult> Details(string id)
        {
            var order = await _orderService.GetOrderByIdForStaffAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // ✅ UC_61: Approve / Reject Order
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string id, string actionType)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(actionType))
                return Json(new { success = false, message = "Invalid parameters." });

            var staffId = "STF001"; // => sau này lấy từ Session/Login
            bool result = false;
            string newStatus = "";

            switch (actionType.ToLower())
            {
                case "approve":
                    result = await _orderService.UpdateOrderStatusAsync(id, "Approved", staffId);
                    newStatus = "Approved";
                    break;
                case "reject":
                    result = await _orderService.UpdateOrderStatusAsync(id, "Cancelled", staffId);
                    newStatus = "Cancelled";
                    break;
                default:
                    return Json(new { success = false, message = "Unknown action type." });
            }

            if (!result)
                return Json(new { success = false, message = $"Failed to {actionType} order." });

            return Json(new
            {
                success = true,
                message = $"Order {newStatus.ToLower()} successfully.",
                newStatus
            });
        }

        // ✅ UC_62: Edit order
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
