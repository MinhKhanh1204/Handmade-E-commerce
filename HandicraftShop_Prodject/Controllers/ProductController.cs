using HandicraftShop_Prodject.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace HandicraftShop_Prodject.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        public IActionResult Index(string? search, int? categoryId, int page = 1)
        {
            var categories = _categoryService.GetAllCategories();
            if (!categoryId.HasValue)
            {
                categoryId = categories.First().CategoryId;
            }
                
            var result = _productService.GetPagedProducts(search, categoryId, page, 6);
            
            var model = new ProductViewModel
            {
                Products = result,
                CategoryId = categoryId,
                Search = search,
                Categories = categories
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductListPartial", result);
            }

            return View(model);
        }

        public IActionResult Detail(string id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound();
            return View(product);
        }
    }
}
