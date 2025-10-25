using BussinessObject;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using DataAccessObject;

namespace Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(MyStoreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            return await _dbSet
                .Include(c => c.CustomerNavigation)
                .Where(c => c.Status == "Active")
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            return await _dbSet
                .Where(c => (c.FullName != null && c.FullName.Contains(keyword)) ||
                           (c.Phone != null && c.Phone.Contains(keyword)) ||
                           (c.Address != null && c.Address.Contains(keyword)) ||
                           (c.Status != null && c.Status.Contains(keyword)) ||
                           c.CustomerId.Contains(keyword))
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerWithOrdersAsync(string customerId)
        {
            return await _dbSet
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _dbSet
                .Include(c => c.CustomerNavigation)
                .AnyAsync(c => c.CustomerNavigation != null && c.CustomerNavigation.Email == email);
        }

        public async Task<bool> IsPhoneExistsAsync(string phone)
        {
            return await _dbSet.AnyAsync(c => c.Phone == phone);
        }
    }
}
