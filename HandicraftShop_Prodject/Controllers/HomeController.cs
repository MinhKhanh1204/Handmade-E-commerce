using Microsoft.AspNetCore.Mvc;
using Services;

namespace HandicraftShop_Prodject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            var model = _productService.GetTop4PromotionProducts();
            return View(model);
        }
    }
}
