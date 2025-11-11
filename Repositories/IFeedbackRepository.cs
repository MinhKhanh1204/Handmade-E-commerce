using BussinessObject;

namespace Repositories
{
    public interface IFeedbackRepository
    {
        Task<Feedback?> GetByIdAsync(int feedbackId);
        Task<Feedback?> GetByCustomerAndProductAsync(string customerId, string productId);
        Task<IEnumerable<Feedback>> GetByProductIdAsync(string productId);
        Task<IEnumerable<Feedback>> GetByCustomerIdAsync(string customerId);
        Task AddAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(int feedbackId);
    }
}