using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessObject;
using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services;

namespace HandicraftShop_Prodject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // UC_33: View orders by customer (tạm dùng CustomerID giả lập)
        public async Task<IActionResult> Index(string? customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                // Giả định lấy từ session hoặc login user
                customerId = "CUS002";
            }

            var orders = await _orderService.SearchOrdersAsync(customerId, null, null, null, null, "pending");
            return View(orders);
        }

        // UC_34: Search orders
        [HttpGet]
        public async Task<IActionResult> Search(string? orderId, DateTime? fromDate, DateTime? toDate, string? paymentStatus, string? status)
        {
            //var customerId = HttpContext.Session.GetString("CustomerId");
            var customerId = "CUS002"; ;
            if (string.IsNullOrEmpty(customerId))
                return Unauthorized();

            // Gọi service để lấy danh sách đơn theo điều kiện
            var results = await _orderService.SearchOrdersAsync(customerId, orderId, fromDate, toDate, paymentStatus, status);

            // AJAX request → trả partial
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_OrderListPartial", results);

            // Truy cập trực tiếp → trả view đầy đủ
            return View("Index", results);
        }


        // UC_35: View order details
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // UC_36: Cancel order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string id, string reason)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            bool success = await _orderService.CancelOrderAsync(id, reason);
            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to cancel this order.";
            }
            else
            {
                TempData["SuccessMessage"] = "Order has been cancelled successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        //public async Task<IActionResult> Index(
        //string? orderId,
        //string? shippingStatus,
        //string? paymentStatus,
        //DateTime? fromDate,
        //DateTime? toDate)
        //{
        //    // 🧠 Lấy CustomerID từ Session (hoặc Claims)
        //    //var customerId = HttpContext.Session.GetString("CustomerId");
        //    var customerId = "CUS001";
        //    if (string.IsNullOrEmpty(customerId))
        //    {
        //        return RedirectToAction("Login", "Account"); // chưa đăng nhập
        //    }

        //    IEnumerable<Order> orders = await _orderService.GetOrdersByCustomerAsync(customerId);

        //    if (!string.IsNullOrEmpty(orderId))
        //        orders = orders.Where(o => o.OrderId.Contains(orderId, StringComparison.OrdinalIgnoreCase));

        //    if (!string.IsNullOrEmpty(shippingStatus))
        //        orders = orders.Where(o => o.ShippingStatus == shippingStatus);

        //    if (!string.IsNullOrEmpty(paymentStatus))
        //        orders = orders.Where(o => o.PaymentStatus == paymentStatus);

        //    if (fromDate.HasValue)
        //        orders = orders.Where(o => o.OrderDate >= fromDate.Value);

        //    if (toDate.HasValue)
        //        orders = orders.Where(o => o.OrderDate <= toDate.Value);


        //    // 🧾 Lưu lại bộ lọc để giữ trạng thái form
        //    ViewBag.OrderId = orderId;
        //    ViewBag.ShippingStatus = shippingStatus;
        //    ViewBag.PaymentStatus = paymentStatus;
        //    ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
        //    ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

        //    return View(orders);
        //}
    }
}