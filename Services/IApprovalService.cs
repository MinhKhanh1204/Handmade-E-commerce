using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessObject;

namespace Services
{
    public interface IApprovalService
    {
        /// <summary>
        /// Duyệt (approve) một entity (Product/Category/Article/Voucher)
        /// </summary>
        Task ApproveAsync(string entityType, string id, string approvedBy);
        Task RejectAsync(string entityType, string id, string rejectedBy);

        /// <summary>
        /// Lấy danh sách các entity có trạng thái Pending
        /// </summary>
        Task<List<Product>> GetPendingProductsAsync();
        Task<List<Category>> GetPendingCategoriesAsync();
        Task<List<Article>> GetPendingArticlesAsync();
        Task<List<Voucher>> GetPendingVouchersAsync();
    }
}
