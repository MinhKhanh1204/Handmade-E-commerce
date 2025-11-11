using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class OrderRepository
    {
        private readonly MyStoreContext _context;

        public OrderRepository(MyStoreContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            // Generate OrderId
            order.OrderId = $"ORD{DateTime.Now:yyyyMMddHHmmss}";
            order.OrderDate = DateTime.Now;
            order.UpdatedAt = DateTime.Now;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task UpdatePaymentStatusAsync(string orderId, string paymentMethod, string status, string note)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.PaymentMethod = paymentMethod;
                order.PaymentStatus = status;
                order.Note = note;
                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// ✅ Kiểm tra customer đã mua sản phẩm này chưa
        /// </summary>
        public async Task<bool> HasCustomerPurchasedProductAsync(string customerId, string productId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId && o.PaymentStatus == "Paid")
                .AnyAsync(o => o.OrderItems.Any(oi => oi.ProductId == productId));
        }
    }
}