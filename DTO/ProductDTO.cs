using System;
using System.Collections.Generic;
using System.Linq;
using BussinessObject;

namespace DTO 
{
    public class ProductDTO
    {
        public string ProductId { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? Material { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public int? StockQuantity { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Status { get; set; }

        public List<string> ImageUrls { get; set; } = new();
        public string? MainImageUrl => ImageUrls.Count > 0 ? ImageUrls[0] : null;

        // ✅ Map từ entity Product sang DTO
        public static ProductDTO FromEntity(Product p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            return new ProductDTO
            {
                ProductId = p.ProductId ?? string.Empty,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName,
                ProductName = p.ProductName,
                Description = p.Description,
                Material = p.Material,
                Price = p.Price,
                Discount = p.Discount,
                StockQuantity = p.StockQuantity,
                CreatedAt = p.CreatedAt,
                Status = p.Status,
                ImageUrls = p.ProductImages?.Select(pi => pi.ImageUrl ?? string.Empty).ToList()
                            ?? new List<string>()
            };
        }
    }
}
