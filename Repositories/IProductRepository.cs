using BussinessObject;
using System.Collections.Generic;

namespace Repositories
{
    public interface IProductRepository
    {
        List<Product> GetProducts();
        Product? GetProductById(string id);
        void SaveProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(string productId);
    }
}
