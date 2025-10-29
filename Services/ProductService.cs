using BussinessObject;
using DTO;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public PagedResult<ProductDTO> GetPagedProducts(string? search, int? categoryId, int page, int pageSize)
        {
            var query = _productRepository.GetAllProducts();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.ProductName.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = query
                .OrderByDescending(p => p.ProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsEnumerable()
                .Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    Discount = p.Discount,
                    CategoryName = p.Category?.CategoryName,
                    ImageUrl = p.ProductImages.FirstOrDefault(img => img.IsMain == true)?.ImageUrl
                                   ?? p.ProductImages.FirstOrDefault()?.ImageUrl
                                   ?? "/images/no-image.png",
                    AverageRating = p.Feedbacks.Any()
                            ? Math.Round(p.Feedbacks.Average(f => f.Rating ?? 0), 1)
                            : 0
                })
                .ToList();

            return new PagedResult<ProductDTO>
            {
                Items = items,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public Product? GetProductById(string productId)
        {
            return _productRepository.GetProductById(productId);
        }
    }
}
