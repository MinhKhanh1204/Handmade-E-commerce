using BussinessObject;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> GetAllProducts();
        Product? GetProductById(string productId);
        List<Product> GetProducts();
        void SaveProduct(Product product);
        void UpdateProduct(Product product);
        void AddProduct(Product product);
        void DeleteProduct(string productId);
    }
}