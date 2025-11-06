using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessObject;

namespace Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        // UC_33: View all orders of a customer
        Task<List<Order>> GetOrdersByCustomerAsync(string customerId);

        // UC_34: Search orders by filters
        Task<IEnumerable<Order>> SearchOrdersAsync(string customerId, string? orderId, DateTime? from, DateTime? to, string? paymentStatus, string? tabStatus);

        // UC_35: View details of a specific order

        Task<Order?> GetOrderByIdAsync(string orderId);

        // UC_36: Cancel an order
        Task<bool> CancelOrderAsync(string orderId, string cancelReason);

        // UC_59: View all orders
        Task<IEnumerable<Order>> GetAllOrdersForStaffAsync();

        // UC_60: Search orders
        Task<IEnumerable<Order>> SearchOrdersForStaffAsync(string? orderId, string? customerName, DateTime? from, DateTime? to, string? shippingStatus, string? paymentStatus);

        // UC_61: Approve order
        Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus, string staffId);

        // UC_62: Edit order
        Task<bool> UpdateOrderAsync(Order updatedOrder);

        //UC_63: View details of a specific order for staff
        Task<Order?> GetOrderByIdForStaffAsync(string orderId);
    }
}
