using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MyStoreContext _context;

        public ProductRepository(MyStoreContext context)
        {
            _context = context;
        }

        public IQueryable<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Feedbacks)
                .Where(p => p.Status == "Active")
                .AsQueryable();
        }

        public Product? GetProductById(string productId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Feedbacks)
                    .ThenInclude(f => f.Customer)
                        .ThenInclude(c => c.CustomerNavigation)
                .Include(p => p.Feedbacks)
                    .ThenInclude(f => f.FeedbackImages)
                .FirstOrDefault(p => p.ProductId == productId);
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        /// <summary>
        /// ✅ Update product (ví dụ: cập nhật stock quantity)
        /// </summary>
        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void DeleteProduct(string productId)
        {
            var product = _context.Products.Find(productId);
            if (product != null)
            {
                product.Status = "Inactive"; // Soft delete
                _context.SaveChanges();
            }
        }
    }
}