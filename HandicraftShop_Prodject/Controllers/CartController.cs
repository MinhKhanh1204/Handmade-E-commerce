using Microsoft.AspNetCore.Mvc;
using Services;
using HandicraftShop_Prodject.Utils;

namespace HandicraftShop_Prodject.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        private readonly IProductService _productService;
        private readonly VNPayService _vnpay;
        private readonly IConfiguration _config;

        public CartController(
            CartService cartService,
            OrderService orderService,
            IProductService productService,
            VNPayService vnpay,
            IConfiguration config)
        {
            _cartService = cartService;
            _orderService = orderService;
            _productService = productService;
            _vnpay = vnpay;
            _config = config;
        }

        // 🛒 View Cart
        public IActionResult Index()
        {
            var account = AccountUtils.GetUserData(User);
            if (account == null)
                return RedirectToAction("Login", "Auth");

            var cart = _cartService.GetCartByCustomerId(account.AccountId);
            return View("~/Views/Cart/Index.cshtml", cart);
        }

        // ✅ Add to Cart (UC_23)
        [HttpPost]
        public IActionResult Add(string productId, int quantity = 1)
        {
            var account = AccountUtils.GetUserData(User);

            if (account == null)
            {
                TempData["ErrorMessage"] = "Please login to add products to cart!";
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Detail", "Product", new { id = productId }) });
            }

            try
            {
                // ✅ Validate product exists
                if (!_productService.ProductExists(productId))
                {
                    TempData["ErrorMessage"] = "Product not found or unavailable!";
                    return RedirectToAction("Index", "Product");
                }

                // ✅ Validate stock
                if (!_productService.IsProductInStock(productId, quantity))
                {
                    TempData["ErrorMessage"] = "Not enough stock available!";
                    return RedirectToAction("Detail", "Product", new { id = productId });
                }

                // Add to cart
                _cartService.AddToCart(account.AccountId, productId, quantity);

                var product = _productService.GetProductById(productId);
                TempData["SuccessMessage"] = $"Added {product?.ProductName} to cart successfully!";
                return RedirectToAction("Detail", "Product", new { id = productId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Detail", "Product", new { id = productId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error adding to cart: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to add product to cart!";
                return RedirectToAction("Detail", "Product", new { id = productId });
            }
        }

        [HttpPost]
        public IActionResult Edit(int cartItemId, int quantity)
        {
            var account = AccountUtils.GetUserData(User);
            if (account == null)
                return RedirectToAction("Login", "Auth");

            try
            {
                _cartService.UpdateCartItem(cartItemId, quantity);
                TempData["SuccessMessage"] = "Cart updated successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating cart: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to update cart!";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int cartItemId)
        {
            var account = AccountUtils.GetUserData(User);
            if (account == null)
                return RedirectToAction("Login", "Auth");

            try
            {
                _cartService.DeleteCartItem(cartItemId);
                TempData["SuccessMessage"] = "Item removed from cart!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error deleting cart item: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to remove item!";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var account = AccountUtils.GetUserData(User);
            if (account == null)
                return RedirectToAction("Login", "Auth");

            var cart = _cartService.GetCartByCustomerId(account.AccountId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Index", "Product");
            }

            return View("~/Views/Order/Checkout.cshtml", cart);
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutSubmit(string paymentMethod, string shippingAddress)
        {
            var account = AccountUtils.GetUserData(User);
            if (account == null)
                return RedirectToAction("Login", "Auth");

            try
            {
                var order = await _orderService.CreateOrderFromCartAsync(account.AccountId, paymentMethod, shippingAddress);

                // ✅ VNPay
                if (paymentMethod == "VNPay")
                {
                    var returnUrl = Url.Action("VNPayReturn", "Order", null, Request.Scheme);
                    var clientIp = VNPayService.GetIpAddress(HttpContext);
                    var url = _vnpay.CreatePaymentUrl(order.OrderId, order.TotalAmount ?? 0M, returnUrl, clientIp);
                    return Redirect(url);
                }

                // ✅ COD
                if (paymentMethod == "COD")
                {
                    await _orderService.UpdatePaymentAsync(order.OrderId, "COD", "Pending", "");
                    return RedirectToAction("Confirmation", "Order", new { id = order.OrderId });
                }

                TempData["ErrorMessage"] = "Invalid payment method!";
                return RedirectToAction("Checkout");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Checkout");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Checkout error: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to create order!";
                return RedirectToAction("Checkout");
            }
        }
    }
}