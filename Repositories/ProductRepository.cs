using BussinessObject;
using DataAccessObject;
using DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.Feedbacks)
                .Where(p => p.Status == "Active");
        }

        public Product? GetProductById(string productId)
        {
            return _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.Feedbacks)
                    .ThenInclude(f => f.Customer)          // include Customer
                    .ThenInclude(c => c.CustomerNavigation) // include Account
                .Include(p => p.Feedbacks)
                    .ThenInclude(f => f.FeedbackImages)    // include FeedbackImages
                .FirstOrDefault(p => p.ProductId == productId);
        }
    }
}
