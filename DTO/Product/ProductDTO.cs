using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ProductDTO
    {
        public string ProductId { get; set; } = null!;
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }
        public double AverageRating { get; set; }
    }
}
