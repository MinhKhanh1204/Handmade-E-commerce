using Microsoft.AspNetCore.Mvc;
using Services;
using HandicraftShop_Prodject.Utils;

namespace HandicraftShop_Prodject.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly VNPayService _vnpayService;

        public OrderController(OrderService orderService, VNPayService vnpayService)
        {
            _orderService = orderService;
            _vnpayService = vnpayService;
        }

        // ✅ Trang xác nhận đơn hàng
        public async Task<IActionResult> Confirmation(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return View("~/Views/Order/Confirmation.cshtml", order);
        }

        // ✅ Callback từ VNPay
        [HttpGet]
        public async Task<IActionResult> VNPayReturn()
        {
            try
            {
                var vnp_SecureHash = Request.Query["vnp_SecureHash"].ToString();
                var vnp_ResponseCode = Request.Query["vnp_ResponseCode"].ToString();
                var vnp_TxnRef = Request.Query["vnp_TxnRef"].ToString();

                Console.WriteLine($"=== VNPay Callback ===");
                Console.WriteLine($"Full URL: {Request.QueryString}");

                // Validate chữ ký
                if (!_vnpayService.ValidateSignature(Request.Query, vnp_SecureHash))
                {
                    TempData["ErrorMessage"] = "Chữ ký không hợp lệ!";
                    return RedirectToAction("Index", "Home");
                }

                var orderId = vnp_TxnRef; // Không cần split vì không thêm timestamp nữa

                if (vnp_ResponseCode == "00")
                {
                    await _orderService.UpdatePaymentAsync(orderId, "VNPay", "Paid",
                        $"TransactionNo: {Request.Query["vnp_TransactionNo"]}");
                    TempData["SuccessMessage"] = "Thanh toán VNPay thành công!";
                }
                else
                {
                    await _orderService.UpdatePaymentAsync(orderId, "VNPay", "Failed",
                        $"ResponseCode: {vnp_ResponseCode}");
                    TempData["ErrorMessage"] = "Thanh toán VNPay thất bại!";
                }

                return RedirectToAction("Confirmation", new { id = orderId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ VNPay ERROR: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }

        // ✅ Callback từ MoMo (User redirect về)
        [HttpGet]
        public async Task<IActionResult> MoMoReturn()
        {
            try
            {
                var orderId = Request.Query["orderId"].ToString();
                var resultCode = Request.Query["resultCode"].ToString();
                var message = Request.Query["message"].ToString();
                var transId = Request.Query["transId"].ToString();

                Console.WriteLine("=== MoMo Return Callback ===");
                Console.WriteLine($"OrderId: {orderId}");
                Console.WriteLine($"ResultCode: {resultCode}");
                Console.WriteLine($"Message: {message}");
                Console.WriteLine($"TransId: {transId}");
                Console.WriteLine("============================");

                if (resultCode == "0") // Thành công
                {
                    await _orderService.UpdatePaymentAsync(orderId, "MoMo", "Paid",
                        $"MoMo TransId: {transId}");
                    TempData["SuccessMessage"] = "Thanh toán MoMo thành công!";
                }
                else
                {
                    await _orderService.UpdatePaymentAsync(orderId, "MoMo", "Failed",
                        $"MoMo ResultCode: {resultCode}");
                    TempData["ErrorMessage"] = $"Thanh toán MoMo thất bại: {message}";
                }

                return RedirectToAction("Confirmation", new { id = orderId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MoMo Return ERROR: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xử lý thanh toán MoMo!";
                return RedirectToAction("Index", "Home");
            }
        }

        // ✅ IPN từ MoMo (Server-to-Server notification)
        [HttpPost]
        public async Task<IActionResult> MoMoNotify()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                Console.WriteLine("=== MoMo IPN Notification ===");
                Console.WriteLine($"Body: {body}");

                // Parse JSON
                var json = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(body);

                if (json != null)
                {
                    var orderId = json["orderId"]?.ToString();
                    var resultCode = json["resultCode"]?.ToString();
                    var transId = json["transId"]?.ToString();

                    Console.WriteLine($"OrderId: {orderId}");
                    Console.WriteLine($"ResultCode: {resultCode}");
                    Console.WriteLine($"TransId: {transId}");

                    if (resultCode == "0" && !string.IsNullOrEmpty(orderId))
                    {
                        await _orderService.UpdatePaymentAsync(orderId, "MoMo", "Paid",
                            $"MoMo TransId: {transId}");
                        Console.WriteLine($"✅ Order {orderId} updated to Paid");
                    }
                }

                Console.WriteLine("=============================");

                // Trả về response cho MoMo
                return Ok(new
                {
                    message = "Success",
                    resultCode = 0
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MoMo IPN ERROR: {ex.Message}");
                return Ok(new
                {
                    message = "Error",
                    resultCode = -1
                });
            }
        }
    }
}