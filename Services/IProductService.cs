using BussinessObject;
using DTO;

namespace Services
{
    public interface IProductService
    {
        Product? GetProductById(string productId);
        PagedResult<ProductDTO> GetPagedProducts(string? search, int? categoryId, int page, int pageSize);

        // ✅ Thêm các method kiểm tra
        bool ProductExists(string productId);
        bool IsProductInStock(string productId, int quantity);
        void UpdateStockQuantity(string productId, int quantity);
    }
}