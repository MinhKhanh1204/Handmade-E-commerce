using System.Collections.Generic;
using System.Threading.Tasks;
using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IApprovalRepository _approvalRepository;
        private readonly MyStoreContext _context;

        public ApprovalService(IApprovalRepository approvalRepository, MyStoreContext context)
        {
            _approvalRepository = approvalRepository;
            _context = context;
        }

        public async Task ApproveAsync(string entityType, string id, string approvedBy)
        {
            await _approvalRepository.UpdateStatusAsync(entityType, id, "Active", approvedBy);
        }

        public async Task RejectAsync(string entityType, string id, string rejectedBy)
        {
            await _approvalRepository.UpdateStatusAsync(entityType, id, "Rejected", rejectedBy);
        }

        public async Task<List<Product>> GetPendingProductsAsync()
        {
            return await _context.Products
                .Where(p => p.Status == "Pending")
                .ToListAsync();
        }

        public async Task<List<Category>> GetPendingCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.Status == "Pending")
                .ToListAsync();
        }

        public async Task<List<Article>> GetPendingArticlesAsync()
        {
            return await _context.Articles
                .Where(a => a.Status == "Pending")
                .ToListAsync();
        }

        public async Task<List<Voucher>> GetPendingVouchersAsync()
        {
            return await _context.Vouchers
                .Where(v => v.IsActive == false) // hoặc dùng Status nếu có
                .ToListAsync();
        }
    }
}
