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
        // UC_33: View orders
        public async Task<List<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            return await _orderRepo.GetOrdersByCustomerAsync(customerId);
        }

        // UC_34: Search orders
        public Task<IEnumerable<Order>> SearchOrdersAsync(string customerId, string? orderId, DateTime? fromDate, DateTime? toDate, string? paymentStatus, string? tabStatus)
        {
            return _orderRepo.SearchOrdersAsync(customerId, orderId, fromDate, toDate, paymentStatus, tabStatus);
        }

        // UC_35: View order details

        // UC_36: Cancel order
        public async Task<bool> CancelOrderAsync(string orderId, string cancelReason)
        {
            return await _orderRepo.CancelOrderAsync(orderId, cancelReason);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersForStaffAsync()
        {
            return await _orderRepo.GetAllOrdersForStaffAsync();
        }

        public async Task<IEnumerable<Order>> SearchOrdersForStaffAsync(string? orderId, string? customerName, DateTime? from, DateTime? to, string? shippingStatus, string? paymentStatus)
        {
            return await _orderRepo.SearchOrdersForStaffAsync(orderId, customerName, from, to, shippingStatus, paymentStatus);
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus, string staffId)
        {
            return await _orderRepo.UpdateOrderStatusAsync(orderId, newStatus, staffId);
        }

        public async Task<bool> UpdateOrderAsync(Order updatedOrder)
        {
            return await _orderRepo.UpdateOrderAsync(updatedOrder);
        }

        public async Task<Order?> GetOrderByIdForStaffAsync(string orderId)
        {
            return await _orderRepo.GetOrderByIdForStaffAsync(orderId);
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