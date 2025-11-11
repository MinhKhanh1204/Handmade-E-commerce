using BussinessObject;

namespace Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> GetAllProducts();
        Product? GetProductById(string productId);
        void AddProduct(Product product);
        void UpdateProduct(Product product); // ✅ Thêm method này
        void DeleteProduct(string productId);
    }
}