using System.Collections.Generic;
using System.Linq;
using BussinessObject;
using DTO;
using Repositories;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public List<Product> GetProducts() => _repo.GetProducts();
        public Product? GetProductById(string id) => _repo.GetProductById(id);
        public void SaveProduct(Product product) => _repo.SaveProduct(product);
        public void UpdateProduct(Product product) => _repo.UpdateProduct(product);
        public void DeleteProduct(string productId) => _repo.DeleteProduct(productId);

        // Map to DTO
        public List<ProductDTO> GetProductDTOs()
        {
            var products = _repo.GetProducts();
            return products.Select(p => ProductDTO.FromEntity(p)).ToList();
        }
    }
}
