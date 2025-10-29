using BussinessObject;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IProductService
    {
        Product? GetProductById(string productId);

        PagedResult<ProductDTO> GetPagedProducts(string? search, int? categoryId, int page, int pageSize);
    }
}
