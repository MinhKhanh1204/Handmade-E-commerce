using Microsoft.AspNetCore.Mvc;
using Services;
using HandicraftShop_Prodject.Utils;
using BussinessObject;

namespace HandicraftShop_Prodject.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        private readonly MoMoService _momo;
        private readonly VNPayService _vnpay;
        private readonly IConfiguration _config;

        public CartController(
            CartService cartService,
            OrderService orderService,
            MoMoService momo,
            VNPayService vnpay,
            IConfiguration config)
        {
            _cartService = cartService;
            _orderService = orderService;
            _momo = momo;
            _vnpay = vnpay;
            _config = config;
        }

        // 🛒 Xem giỏ hàng
        public async Task<IActionResult> Index()
        {
            var account = AccountUtils.GetUserData(User);
            if (account == null)
                return RedirectToAction("Login", "Auth");

            var cart = await _cartService.GetCartByCustomerIdAsync(account.AccountId);

            return View("~/Views/Cart/Index.cshtml", cart);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int cartItemId, int quantity)
        {
            var account = AccountUtils.GetUserData(User);
            if (account == null)
                return RedirectToAction("Login", "Auth");

            await _cartService.UpdateCartItemAsync(cartItemId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int cartItemId)
        {
            var account = AccountUtils.GetUserData(User);
            if (account == null)
                return RedirectToAction("Login", "Auth");

            await _cartService.DeleteCartItemAsync(cartItemId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var account = AccountUtils.GetUserData(User);
            if (account == null)
                return RedirectToAction("Login", "Auth");

            var cart = await _cartService.GetCartByCustomerIdAsync(account.AccountId);
            return View("~/Views/Order/Checkout.cshtml", cart);
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutSubmit(string paymentMethod, string shippingAddress)
        {
            var account = AccountUtils.GetUserData(User);
            if (account == null)
                return RedirectToAction("Login", "Auth");

            var order = await _orderService.CreateOrderFromCartAsync(account.AccountId, paymentMethod, shippingAddress);

            if (paymentMethod == "MoMo")
            {
                var cfg = _config.GetSection("MoMo");
                var (ok, url, id, msg) = await _momo.CreatePaymentAsync(
                    order.TotalAmount ?? 0M,
                    order.OrderId,
                    $"Thanh toán đơn {order.OrderId}",
                    cfg["ReturnUrl"],
                    cfg["NotifyUrl"]);
                if (ok) return Redirect(url);
            }

            if (paymentMethod == "VNPay")
            {
                var cfg = _config.GetSection("VNPay");
                var url = _vnpay.CreatePaymentUrl(order.OrderId, order.TotalAmount ?? 0M, cfg["ReturnUrl"]);
                return Redirect(url);

            }

            // COD
            await _orderService.UpdatePaymentAsync(order.OrderId, "COD", "Pending", "");
            return RedirectToAction("Confirmation", "Order", new { id = order.OrderId });
        }
    }
}
