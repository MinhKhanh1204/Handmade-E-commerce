using BussinessObject;

namespace Services
{
    public interface IFeedbackService
    {
        Task<Feedback?> GetFeedbackByIdAsync(int feedbackId);
        Task<Feedback?> GetFeedbackByCustomerAndProductAsync(string customerId, string productId);
        Task<IEnumerable<Feedback>> GetFeedbacksByProductIdAsync(string productId);
        Task<IEnumerable<Feedback>> GetFeedbacksByCustomerIdAsync(string customerId);
        Task AddFeedbackAsync(Feedback feedback);
        Task UpdateFeedbackAsync(Feedback feedback);
        Task DeleteFeedbackAsync(int feedbackId);
        Task<double> GetAverageRatingByProductIdAsync(string productId);
    }
}