using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessObject;
using Repositories;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // UC_33: View orders
        public async Task<List<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            return await _orderRepository.GetOrdersByCustomerAsync(customerId);
        }

        // UC_34: Search orders
        public Task<IEnumerable<Order>> SearchOrdersAsync(string customerId, string? orderId, DateTime? fromDate, DateTime? toDate, string? paymentStatus, string? tabStatus)
        {
            return _orderRepository.SearchOrdersAsync(customerId, orderId, fromDate, toDate, paymentStatus, tabStatus);
        }

        // UC_35: View order details
        public async Task<Order?> GetOrderByIdAsync(string orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        // UC_36: Cancel order
        public async Task<bool> CancelOrderAsync(string orderId, string cancelReason)
        {
            return await _orderRepository.CancelOrderAsync(orderId, cancelReason);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersForStaffAsync()
        {
            return await _orderRepository.GetAllOrdersForStaffAsync();
        }

        public async Task<IEnumerable<Order>> SearchOrdersForStaffAsync(string? orderId, string? customerName, DateTime? from, DateTime? to, string? shippingStatus, string? paymentStatus)
        {
            return await _orderRepository.SearchOrdersForStaffAsync(orderId, customerName, from, to, shippingStatus, paymentStatus);
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus, string staffId)
        {
            return await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, staffId);
        }

        public async Task<bool> UpdateOrderAsync(Order updatedOrder)
        {
            return await _orderRepository.UpdateOrderAsync(updatedOrder);
        }

        public async Task<Order?> GetOrderByIdForStaffAsync(string orderId)
        {
            return await _orderRepository.GetOrderByIdForStaffAsync(orderId);
        }
    }
}
