using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BussinessObject;
using Repositories;

namespace Services
{
	public class CustomerService : ICustomerService
	{
		private readonly ICustomerRepository _customerRepository;

		public CustomerService(ICustomerRepository customerRepository)
		{
			_customerRepository = customerRepository;
		}

		public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
		{
			return await _customerRepository.GetAllAsync();
		}

		public async Task<Customer?> GetCustomerByIdAsync(string customerId)
		{
			return await _customerRepository.GetByIdAsync(customerId);
		}

		public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
		{
			return await _customerRepository.GetActiveCustomersAsync();
		}

		public async Task<IEnumerable<Customer>> SearchCustomersAsync(string keyword)
		{
			return await _customerRepository.SearchCustomersAsync(keyword);
		}

		public async Task<Customer?> GetCustomerDetailsAsync(string customerId)
		{
			return await _customerRepository.GetCustomerWithOrdersAsync(customerId);
		}

		public async Task<Customer> CreateCustomerAsync(Customer customer)
		{
			if (string.IsNullOrWhiteSpace(customer.FullName))
				throw new ArgumentException("Customer name is required");

			if (string.IsNullOrWhiteSpace(customer.Phone))
				throw new ArgumentException("Phone is required");

			if (await _customerRepository.IsPhoneExistsAsync(customer.Phone))
				throw new InvalidOperationException("Phone number already exists");

			// Generate next CustomerId like CUS001
			var all = await _customerRepository.GetAllAsync();
			var maxId = all
				.Where(c => c.CustomerId.StartsWith("CUS"))
				.Select(c => int.TryParse(c.CustomerId.Substring(3), out var id) ? id : 0)
				.DefaultIfEmpty(0)
				.Max();

			customer.CustomerId = $"CUS{(maxId + 1):D3}";
			customer.Status = customer.Status ?? "Active";

			return await _customerRepository.AddAsync(customer);
		}

		public async Task<Customer> UpdateCustomerAsync(Customer customer)
		{
			var existing = await _customerRepository.GetByIdAsync(customer.CustomerId);
			if (existing == null)
				throw new ArgumentException("Customer not found");

			if (string.IsNullOrWhiteSpace(customer.FullName))
				throw new ArgumentException("Customer name is required");

			if (string.IsNullOrWhiteSpace(customer.Phone))
				throw new ArgumentException("Phone is required");

			var phoneExists = await _customerRepository.IsPhoneExistsAsync(customer.Phone);
			if (phoneExists && existing.Phone != customer.Phone)
				throw new InvalidOperationException("Phone number already exists");

			existing.FullName = customer.FullName;
			existing.DateOfBirth = customer.DateOfBirth;
			existing.Gender = customer.Gender;
			existing.Phone = customer.Phone;
			existing.Address = customer.Address;
			existing.Status = customer.Status;

			await _customerRepository.UpdateAsync(existing);
			return existing;
		}

		public async Task<bool> DeleteCustomerAsync(string customerId)
		{
			var existing = await _customerRepository.GetByIdAsync(customerId);
			if (existing == null) return false;
			existing.Status = "Inactive";
			await _customerRepository.UpdateAsync(existing);
			return true;
		}

		public async Task<bool> IsEmailExistsAsync(string email)
		{
			return await _customerRepository.IsEmailExistsAsync(email);
		}

		public async Task<bool> IsPhoneExistsAsync(string phone)
		{
			return await _customerRepository.IsPhoneExistsAsync(phone);
		}

		public async Task<int> GetTotalCustomersCountAsync()
		{
			return await _customerRepository.CountAsync();
		}
	}
}


