using BussinessObject;
using Repositories;

namespace Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepo;

        public FeedbackService(IFeedbackRepository feedbackRepo)
        {
            _feedbackRepo = feedbackRepo;
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(int feedbackId)
        {
            return await _feedbackRepo.GetByIdAsync(feedbackId);
        }

        public async Task<Feedback?> GetFeedbackByCustomerAndProductAsync(string customerId, string productId)
        {
            return await _feedbackRepo.GetByCustomerAndProductAsync(customerId, productId);
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByProductIdAsync(string productId)
        {
            return await _feedbackRepo.GetByProductIdAsync(productId);
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByCustomerIdAsync(string customerId)
        {
            return await _feedbackRepo.GetByCustomerIdAsync(customerId);
        }

        public async Task AddFeedbackAsync(Feedback feedback)
        {
            if (feedback.Rating < 1 || feedback.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5");
            }

            if (string.IsNullOrWhiteSpace(feedback.Comment))
            {
                throw new ArgumentException("Comment is required");
            }

            feedback.CreatedAt = DateTime.Now;
            feedback.UpdatedAt = DateTime.Now;

            await _feedbackRepo.AddAsync(feedback);
        }

        public async Task UpdateFeedbackAsync(Feedback feedback)
        {
            if (feedback.Rating < 1 || feedback.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5");
            }

            if (string.IsNullOrWhiteSpace(feedback.Comment))
            {
                throw new ArgumentException("Comment is required");
            }

            feedback.UpdatedAt = DateTime.Now;

            await _feedbackRepo.UpdateAsync(feedback);
        }

        public async Task DeleteFeedbackAsync(int feedbackId)
        {
            await _feedbackRepo.DeleteAsync(feedbackId);
        }

        public async Task<double> GetAverageRatingByProductIdAsync(string productId)
        {
            var feedbacks = await _feedbackRepo.GetByProductIdAsync(productId);

            if (!feedbacks.Any())
                return 0;

            return feedbacks.Average(f => f.Rating ?? 0);
        }
    }
}