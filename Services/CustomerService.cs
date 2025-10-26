using BussinessObject;
using Repositories.Interfaces;
using Services.Interfaces;

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
            // Business logic validation
            if (string.IsNullOrWhiteSpace(customer.FullName))
                throw new ArgumentException("Customer name is required");

            if (string.IsNullOrWhiteSpace(customer.Phone))
                throw new ArgumentException("Phone is required");

            // Check if phone already exists
            if (await _customerRepository.IsPhoneExistsAsync(customer.Phone))
                throw new InvalidOperationException("Phone number already exists");

            // Set default values - Generate CustomerId in format CUS001, CUS002, etc.
            var existingCustomers = await _customerRepository.GetAllAsync();
            var maxId = existingCustomers
                .Where(c => c.CustomerId.StartsWith("CUS"))
                .Select(c => int.TryParse(c.CustomerId.Substring(3), out var id) ? id : 0)
                .DefaultIfEmpty(0)
                .Max();
            
            customer.CustomerId = $"CUS{(maxId + 1):D3}";
            customer.Status = "Active";

            return await _customerRepository.AddAsync(customer);
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(customer.CustomerId);
            if (existingCustomer == null)
                throw new ArgumentException("Customer not found");

            // Business logic validation
            if (string.IsNullOrWhiteSpace(customer.FullName))
                throw new ArgumentException("Customer name is required");

            if (string.IsNullOrWhiteSpace(customer.Phone))
                throw new ArgumentException("Phone is required");

            // Check if phone already exists for other customers
            var phoneExists = await _customerRepository.IsPhoneExistsAsync(customer.Phone);
            if (phoneExists && existingCustomer.Phone != customer.Phone)
                throw new InvalidOperationException("Phone number already exists");

            // Update existing customer properties instead of creating new instance
            existingCustomer.FullName = customer.FullName;
            existingCustomer.DateOfBirth = customer.DateOfBirth;
            existingCustomer.Gender = customer.Gender;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.Address = customer.Address;
            existingCustomer.Status = customer.Status;

            await _customerRepository.UpdateAsync(existingCustomer);
            return existingCustomer;
        }

        public async Task<bool> DeleteCustomerAsync(string customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                return false;

            // Soft delete - change status to Inactive
            customer.Status = "Inactive";
            await _customerRepository.UpdateAsync(customer);
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
