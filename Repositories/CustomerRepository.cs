using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
	public class CustomerRepository : ICustomerRepository
	{
		private readonly MyStoreContext _context;

		public CustomerRepository(MyStoreContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Customer>> GetAllAsync()
		{
			return await _context.Customers
				.Include(c => c.CustomerNavigation)
				.ToListAsync();
		}

		public async Task<Customer?> GetByIdAsync(string customerId)
		{
			return await _context.Customers
				.Include(c => c.CustomerNavigation)
				.FirstOrDefaultAsync(c => c.CustomerId == customerId);
		}

		public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
		{
			return await _context.Customers
				.Include(c => c.CustomerNavigation)
				.Where(c => c.Status == "Active")
				.OrderBy(c => c.FullName)
				.ToListAsync();
		}

		public async Task<IEnumerable<Customer>> SearchCustomersAsync(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return await GetAllAsync();
			}

			return await _context.Customers
				.Include(c => c.CustomerNavigation)
				.Where(c =>
					(c.FullName != null && c.FullName.Contains(keyword)) ||
					(c.Phone != null && c.Phone.Contains(keyword)) ||
					(c.Address != null && c.Address.Contains(keyword)) ||
					(c.Status != null && c.Status.Contains(keyword)) ||
					c.CustomerId.Contains(keyword))
				.OrderBy(c => c.FullName)
				.ToListAsync();
		}

		public async Task<Customer?> GetCustomerWithOrdersAsync(string customerId)
		{
			return await _context.Customers
				.Include(c => c.CustomerNavigation)
				.Include(c => c.Orders)
					.ThenInclude(o => o.OrderItems)
						.ThenInclude(oi => oi.Product)
				.FirstOrDefaultAsync(c => c.CustomerId == customerId);
		}

		public async Task<bool> IsEmailExistsAsync(string email)
		{
			return await _context.Customers
				.Include(c => c.CustomerNavigation)
				.AnyAsync(c => c.CustomerNavigation != null && c.CustomerNavigation.Email == email);
		}

		public async Task<bool> IsPhoneExistsAsync(string phone)
		{
			return await _context.Customers.AnyAsync(c => c.Phone == phone);
		}

		public async Task<Customer> AddAsync(Customer customer)
		{
			_context.Customers.Add(customer);
			await _context.SaveChangesAsync();
			return customer;
		}

		public async Task UpdateAsync(Customer customer)
		{
			_context.Customers.Update(customer);
			await _context.SaveChangesAsync();
		}

		public async Task<int> CountAsync()
		{
			return await _context.Customers.CountAsync();
		}
	}
}


