using BussinessObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
	public interface ICustomerRepository
	{
		Task<IEnumerable<Customer>> GetAllAsync();
		Task<Customer?> GetByIdAsync(string customerId);
		Task<IEnumerable<Customer>> GetActiveCustomersAsync();
		Task<IEnumerable<Customer>> SearchCustomersAsync(string keyword);
		Task<Customer?> GetCustomerWithOrdersAsync(string customerId);
		Task<bool> IsEmailExistsAsync(string email);
		Task<bool> IsPhoneExistsAsync(string phone);
		Task<Customer> AddAsync(Customer customer);
		Task UpdateAsync(Customer customer);
		Task<int> CountAsync();
	}
}


