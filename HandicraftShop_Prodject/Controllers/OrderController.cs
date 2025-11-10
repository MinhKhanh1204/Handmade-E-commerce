using Microsoft.AspNetCore.Mvc;
using Services;
using HandicraftShop_Prodject.Utils;

namespace HandicraftShop_Prodject.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // ✅ Hiển thị trang xác nhận sau khi thanh toán
        public async Task<IActionResult> Confirmation(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return View("~/Views/Order/Confirmation.cshtml", order);
        }

        // ✅ Callback sau khi thanh toán VNPay hoặc MoMo (sandbox)
        [HttpGet]
        public async Task<IActionResult> PaymentCallback(string orderId, string resultCode = "", string vnp_ResponseCode = "")
        {
            bool success = resultCode == "0" || vnp_ResponseCode == "00";
            await _orderService.UpdatePaymentAsync(orderId, success ? "Paid" : "Failed", "Completed", "");
            return RedirectToAction("Confirmation", new { id = orderId });
        }
    }
}
