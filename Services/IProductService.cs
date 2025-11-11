using BussinessObject;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using DTO;

namespace Services
{
    public interface IProductService
    {
        Product? GetProductById(string productId);
        PagedResult<ProductDTO> GetPagedProducts(string? search, int? categoryId, int page, int pageSize);
        IEnumerable<ProductDTO> GetTop4PromotionProducts();
        List<Product> GetProducts();
        void SaveProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(string productId);

        // thêm method DTO để controller hiển thị (Index)
        List<ProductDTO> GetProductDTOs();

        // ✅ Thêm các method kiểm tra
        bool ProductExists(string productId);
        bool IsProductInStock(string productId, int quantity);
        void UpdateStockQuantity(string productId, int quantity);
    }
}