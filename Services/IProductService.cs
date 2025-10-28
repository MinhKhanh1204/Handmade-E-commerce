using System.Collections.Generic;
using BussinessObject;
using DTO;

namespace Services
{
    public interface IProductService
    {
        List<Product> GetProducts();
        Product? GetProductById(string id);
        void SaveProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(string productId);

        // thêm method DTO để controller hiển thị (Index)
        List<ProductDTO> GetProductDTOs();
    }
}
