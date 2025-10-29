using BussinessObject;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> GetAllProducts();
        Product? GetProductById(string productId);
    }
}
