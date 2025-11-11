using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly MyStoreContext _context;

        public FeedbackRepository(MyStoreContext context)
        {
            _context = context;
        }

        public async Task<Feedback?> GetByIdAsync(int feedbackId)
        {
            return await _context.Feedbacks
                .Include(f => f.Customer)
                    .ThenInclude(c => c.CustomerNavigation)
                .Include(f => f.Product)
                .Include(f => f.FeedbackImages)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
        }

        public async Task<Feedback?> GetByCustomerAndProductAsync(string customerId, string productId)
        {
            return await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.CustomerId == customerId && f.ProductId == productId);
        }

        public async Task<IEnumerable<Feedback>> GetByProductIdAsync(string productId)
        {
            return await _context.Feedbacks
                .Include(f => f.Customer)
                    .ThenInclude(c => c.CustomerNavigation)
                .Include(f => f.FeedbackImages)
                .Where(f => f.ProductId == productId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetByCustomerIdAsync(string customerId)
        {
            return await _context.Feedbacks
                .Include(f => f.Product)
                    .ThenInclude(p => p.ProductImages)
                .Include(f => f.FeedbackImages)
                .Where(f => f.CustomerId == customerId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int feedbackId)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.FeedbackImages)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);

            if (feedback != null)
            {
                // Xóa feedback images trước
                if (feedback.FeedbackImages.Any())
                {
                    _context.FeedbackImages.RemoveRange(feedback.FeedbackImages);
                }

                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }
        }
    }
}