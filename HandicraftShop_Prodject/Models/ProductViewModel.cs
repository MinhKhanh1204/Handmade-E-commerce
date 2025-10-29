using BussinessObject;
using DTO;

namespace HandicraftShop_Prodject.Models
{
    public class ProductViewModel
    {
        public PagedResult<ProductDTO> Products { get; set; } = new PagedResult<ProductDTO>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public int? CategoryId { get; set; }
        public string? Search { get; set; }
    }
}
