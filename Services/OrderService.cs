using System;
using System.Linq;
using System.Threading.Tasks;
using BussinessObject;
using Repositories;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepo;
        private readonly CartRepository _cartRepo;

        public OrderService(OrderRepository orderRepo, CartRepository cartRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
        }

        public async Task<Order> CreateOrderFromCartAsync(string customerId, string paymentMethod, string shippingAddress)
        {
            var cart = await _cartRepo.GetCartByCustomerIdAsync(customerId);
            if (cart == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Giỏ hàng trống!");

            decimal total = 0M;
            var order = new Order
            {
                CustomerId = customerId,
                ShippingAddress = shippingAddress,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Pending",
                ShippingStatus = "Processing",
                OrderItems = new List<OrderItem>()
            };

            foreach (var ci in cart.CartItems)
            {
                var product = ci.Product;
                if (product == null) continue;

                var qty = ci.Quantity ?? 0;
                var unitPrice = product.Price ?? 0M;
                var discount = product.Discount ?? 0M;

                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = ci.ProductId,
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    Discount = discount
                };

                order.OrderItems.Add(orderItem);
                total += (unitPrice - (unitPrice * discount / 100M)) * qty;
            }

            order.TotalAmount = total;

            var created = await _orderRepo.CreateAsync(order);
            await _cartRepo.ClearCartAsync(customerId);

            return created;
        }

        public async Task UpdatePaymentAsync(string orderId, string paymentMethod, string status, string note)
        {
            await _orderRepo.UpdatePaymentStatusAsync(orderId, paymentMethod, status, note);
        }

        public async Task<Order?> GetOrderByIdAsync(string orderId)
        {
            return await _orderRepo.GetByIdAsync(orderId);
        }

        /// <summary>
        /// ✅ Kiểm tra xem customer đã mua sản phẩm này chưa
        /// </summary>
        public async Task<bool> HasCustomerPurchasedProductAsync(string customerId, string productId)
        {
            return await _orderRepo.HasCustomerPurchasedProductAsync(customerId, productId);
        }
    }
}