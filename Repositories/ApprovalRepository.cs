using System.Threading.Tasks;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly MyStoreContext _context;
        public ApprovalRepository(MyStoreContext context)
        {
            _context = context;
        }

        public async Task UpdateStatusAsync(string entityType, string id, string newStatus, string approvedBy)
        {
            switch (entityType.ToLower())
            {
                case "category":
                    if (int.TryParse(id, out int categoryId))
                    {
                        var category = await _context.Categories.FindAsync(categoryId);
                        if (category != null)
                            category.Status = newStatus;
                    }
                    break;

                case "product":
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                    if (product != null)
                        product.Status = newStatus;
                    break;

                case "article":
                    if (int.TryParse(id, out int articleId))
                    {
                        var article = await _context.Articles.FindAsync(articleId);
                        if (article != null)
                            article.Status = newStatus;
                    }
                    break;

                case "voucher":
                    if (int.TryParse(id, out int voucherId))
                    {
                        var voucher = await _context.Vouchers.FindAsync(voucherId);
                        if (voucher != null)
                            voucher.IsActive = newStatus == "Approved";
                    }
                    break;
            }

            await _context.SaveChangesAsync();
        }
    }
}
