using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessObject;
using BussinessObject;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MyStoreContext _context;

        public OrderRepository(MyStoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }


        // UC_33: View orders
        public async Task<List<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }


        // UC_34: Search orders
        public async Task<IEnumerable<Order>> SearchOrdersAsync(string customerId, string? orderId, DateTime? from, DateTime? to, string? paymentStatus, string? tabStatus)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Where(o => o.CustomerId == customerId);

            if (!string.IsNullOrEmpty(orderId))
                query = query.Where(o => o.OrderId.Contains(orderId));

            if (from.HasValue)
                query = query.Where(o => o.OrderDate >= from.Value);

            if (to.HasValue)
                query = query.Where(o => o.OrderDate <= to.Value);

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(o => o.PaymentStatus == paymentStatus);

            // Tab logic
            if (tabStatus == "pending")
                query = query.Where(o => o.ShippingStatus == "Pending" || o.PaymentStatus == "Pending");
            else if (tabStatus == "history")
                query = query.Where(o => o.ShippingStatus == "Delivered" || o.ShippingStatus == "Cancelled");

            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }


        // UC_35: View order (details)
        public async Task<Order?> GetOrderByIdAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Voucher)
                .Include(o => o.Staff)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        // UC_36: Cancel order
        public async Task<bool> CancelOrderAsync(string orderId, string cancelReason)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return false;

            if (order.ShippingStatus == "Cancelled" || order.ShippingStatus == "Delivered")
                return false; // Không thể hủy nếu đã giao hoặc đã hủy

            order.ShippingStatus = "Cancelled";
            order.PaymentStatus = "Refunded";
            order.Note = (order.Note ?? "") + $" [Cancelled: {cancelReason} - {DateTime.Now:yyyy-MM-dd HH:mm}]";
            order.UpdatedAt = DateTime.Now;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersForStaffAsync()
        {
            return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Staff)
            .Include(o => o.Voucher)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        }

        public async Task<IEnumerable<Order>> SearchOrdersForStaffAsync(string? orderId, string? customerName, DateTime? from, DateTime? to, string? shippingStatus, string? paymentStatus)
        {
            var query = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Staff)
            .Include(o => o.Voucher)
            .AsQueryable();

            if (!string.IsNullOrEmpty(orderId))
                query = query.Where(o => o.OrderId.Contains(orderId));

            if (!string.IsNullOrEmpty(customerName))
                query = query.Where(o => o.Customer.FullName.Contains(customerName));

            if (from.HasValue)
                query = query.Where(o => o.OrderDate >= from.Value);

            if (to.HasValue)
                query = query.Where(o => o.OrderDate <= to.Value);

            if (!string.IsNullOrEmpty(shippingStatus))
                query = query.Where(o => o.ShippingStatus == shippingStatus);

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(o => o.PaymentStatus == paymentStatus);

            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus, string staffId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return false;

            order.ShippingStatus = newStatus;
            order.StaffId = staffId;
            order.UpdatedAt = DateTime.Now;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> UpdateOrderAsync(Order updatedOrder)
        {
            var existing = await _context.Orders.FindAsync(updatedOrder.OrderId);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(updatedOrder);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Order?> GetOrderByIdForStaffAsync(string orderId)
        {
            return await _context.Orders 
            .Include(o => o.Staff)
            .Include(o => o.Voucher)
            .Include(o => o.Customer)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
