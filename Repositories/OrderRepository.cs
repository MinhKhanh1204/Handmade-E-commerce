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
            order.OrderId = GenerateOrderId();
            order.OrderDate = DateTime.Now;
            order.UpdatedAt = DateTime.Now;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        private string GenerateOrderId()
        {
            return $"ORD{DateTime.Now:yyyyMMddHHmmss}";
        }

        public async Task<Order?> GetByIdAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task UpdatePaymentStatusAsync(string orderId, string paymentMethod, string paymentStatus, string note)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.PaymentMethod = paymentMethod;
                order.PaymentStatus = paymentStatus;
                order.Note = note;
                order.UpdatedAt = DateTime.Now;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
