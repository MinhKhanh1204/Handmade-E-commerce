using BussinessObject;

namespace Repositories.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();
        Task<IEnumerable<Customer>> SearchCustomersAsync(string keyword);
        Task<Customer?> GetCustomerWithOrdersAsync(string customerId);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsPhoneExistsAsync(string phone);
    }
}
