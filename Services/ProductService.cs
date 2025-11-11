using Azure;
using BussinessObject;
using DTO;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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
                    StockQuantity = p.StockQuantity,
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

        public IEnumerable<ProductDTO> GetTop4PromotionProducts()
        {
            var query = _productRepository.GetAllProducts();
            var items = query
                .Where(p => p.Discount > 0)
                .OrderByDescending(p => p.ProductId)
                .Take(4)
                .AsEnumerable()
                .Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
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

            return items;
        }

        public List<Product> GetProducts() => _productRepository.GetProducts();
        public void SaveProduct(Product product) => _productRepository.SaveProduct(product);
        public void UpdateProduct(Product product) => _productRepository.UpdateProduct(product);
        public void DeleteProduct(string productId) => _productRepository.DeleteProduct(productId);

        // Map to DTO - use GetAllProducts so deleted products are excluded
        public List<ProductDTO> GetProductDTOs()
        {
            var products = _productRepository.GetAllProducts().AsEnumerable();
            return products.Select(p => ProductDTO.FromEntity(p)).ToList();
        }
        /// <summary>
        /// ✅ Kiểm tra product có tồn tại và active không
        /// </summary>
        public bool ProductExists(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return false;

            var product = _productRepository.GetProductById(productId);
            return product != null && product.Status == "Active";
        }

        /// <summary>
        /// ✅ Kiểm tra số lượng trong kho có đủ không
        /// </summary>
        public bool IsProductInStock(string productId, int quantity)
        {
            if (quantity <= 0)
                return false;

            var product = _productRepository.GetProductById(productId);

            if (product == null || product.Status != "Active")
                return false;

            return (product.StockQuantity ?? 0) >= quantity;
        }

        /// <summary>
        /// ✅ Cập nhật số lượng tồn kho (trừ đi khi đặt hàng)
        /// </summary>
        public void UpdateStockQuantity(string productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            var product = _productRepository.GetProductById(productId);

            if (product == null)
                throw new InvalidOperationException("Product not found!");

            if (product.Status != "Active")
                throw new InvalidOperationException("Product is not available!");

            if ((product.StockQuantity ?? 0) < quantity)
                throw new InvalidOperationException($"Not enough stock! Available: {product.StockQuantity}");

            product.StockQuantity = (product.StockQuantity ?? 0) - quantity;
            _productRepository.UpdateProduct(product);
        }
    }
}