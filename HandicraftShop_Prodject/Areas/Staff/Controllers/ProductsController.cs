using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BussinessObject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using DTO;

namespace HandicraftShop_Prodject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;

        public ProductsController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment env)
        {
            _productService = productService;
            _categoryService = categoryService;
            _env = env;
        }

        // ====================== INDEX ======================
        public IActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var productDTOs = _productService.GetProductDTOs().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var lowerSearch = searchString.Trim().ToLower();
                productDTOs = productDTOs.Where(p =>
                    (!string.IsNullOrEmpty(p.ProductName) && p.ProductName!.ToLower().Contains(lowerSearch)) ||
                    (!string.IsNullOrEmpty(p.ProductId) && p.ProductId.ToLower().Contains(lowerSearch)) ||
                    (!string.IsNullOrEmpty(p.CategoryName) && p.CategoryName!.ToLower().Contains(lowerSearch))
                );
            }

            int totalItems = productDTOs.Count();
            int totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);

            var items = productDTOs
                .OrderBy(p => p.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = searchString;

            // return view model: IEnumerable<ProductDTO>
            return View(items);
        }

        // ====================== DETAILS ======================
        public IActionResult Details(string id)
        {
            if (id == null) return NotFound();
            var product = _productService.GetProductById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // ====================== CREATE (GET) ======================
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName");
            return View();
        }

        // ====================== CREATE (POST) ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,ProductName,Description,Material,Price,Discount,StockQuantity,Status")] Product product, IFormFile[] ProductImages)
        {
            if (string.IsNullOrWhiteSpace(product.ProductId))
            {
                if (product.CategoryId.HasValue)
                {
                    product.ProductId = GenerateNewProductId(product.CategoryId.Value);
                }
                else
                {
                    product.ProductId = Guid.NewGuid().ToString("N").Substring(0, 20);
                }
                ModelState.Remove(nameof(Product.ProductId));
            }

            var files = (ProductImages != null && ProductImages.Length > 0)
                        ? ProductImages.ToList()
                        : Request.Form.Files.Where(f => f.Name == "ProductImages").ToList();

            if (files == null || files.Count != 4)
            {
                ModelState.AddModelError("ProductImages", "Please upload exactly 4 product images.");
            }

            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;
                product.ProductImages = new List<ProductImage>();

                var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "products", product.ProductId);
                Directory.CreateDirectory(uploadsRoot);

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file != null && file.Length > 0)
                    {
                        var ext = Path.GetExtension(file.FileName);
                        var fileName = $"{Guid.NewGuid():N}{ext}";
                        var filePath = Path.Combine(uploadsRoot, fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var relativeUrl = $"/uploads/products/{product.ProductId}/{fileName}";

                        product.ProductImages.Add(new ProductImage
                        {
                            ImageUrl = relativeUrl,
                            IsMain = (i == 0)
                        });
                    }
                }

                _productService.SaveProduct(product);
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // ====================== AJAX GET for generating ProductId ======================
        [HttpGet]
        public IActionResult GetProductId(int categoryId)
        {
            var newId = GenerateNewProductId(categoryId);
            return Content(newId, "text/plain");
        }

        private string GenerateNewProductId(int categoryId)
        {
            string prefix = categoryId switch
            {
                1 => "LB",
                2 => "BM",
                3 => "SC",
                4 => "TR",
                5 => "DU",
                _ => "SP"
            };

            var existingIds = _productService.GetProducts()
                .Where(p => p.CategoryId == categoryId && !string.IsNullOrEmpty(p.ProductId) && p.ProductId.StartsWith(prefix))
                .Select(p => p.ProductId!)
                .ToList();

            int maxSeq = 0;
            foreach (var id in existingIds)
            {
                string numberPart = id.Substring(prefix.Length);
                if (int.TryParse(numberPart, out int seq))
                {
                    if (seq > maxSeq) maxSeq = seq;
                }
            }

            int nextSeq = maxSeq + 1;
            string candidate = prefix + nextSeq.ToString("D2");

            while (_productService.GetProductById(candidate) != null)
            {
                nextSeq++;
                candidate = prefix + nextSeq.ToString("D2");
            }

            return candidate;
        }

        // ====================== EDIT (GET) ======================
        public IActionResult Edit(string id)
        {
            var product = _productService.GetProductById(id);
            if (product == null) return NotFound();
            ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // ====================== EDIT (POST) ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Product product, IFormFile[] ProductImages)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var existing = _productService.GetProductById(id);
            if (existing == null) return NotFound();

            existing.ProductName = product.ProductName;
            existing.Description = product.Description;
            existing.Material = product.Material;
            existing.Price = product.Price;
            existing.Discount = product.Discount;
            existing.StockQuantity = product.StockQuantity;
            existing.Status = product.Status;
            existing.CategoryId = product.CategoryId;

            if (ProductImages != null && ProductImages.Length > 0)
            {
                if (ProductImages.Length != 4)
                {
                    ModelState.AddModelError("ProductImages", "Please upload exactly 4 product images.");
                    ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
                    return View(existing);
                }

                var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "products", existing.ProductId);
                if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

                foreach (var oldImg in existing.ProductImages.ToList())
                {
                    var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot",
                                                oldImg.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                }

                existing.ProductImages.Clear();

                for (int i = 0; i < ProductImages.Length; i++)
                {
                    var file = ProductImages[i];
                    if (file != null && file.Length > 0)
                    {
                        var ext = Path.GetExtension(file.FileName);
                        var fileName = $"{Guid.NewGuid():N}{ext}";
                        var filePath = Path.Combine(uploadsRoot, fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var relativeUrl = $"/uploads/products/{existing.ProductId}/{fileName}";

                        existing.ProductImages.Add(new ProductImage
                        {
                            ProductId = existing.ProductId,
                            ImageUrl = relativeUrl,
                            IsMain = (i == 0)
                        });
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
                return View(existing);
            }

            _productService.UpdateProduct(existing);

            return RedirectToAction(nameof(Index));
        }

        // ====================== DELETE (GET) ======================
        public IActionResult Delete(string id)
        {
            var product = _productService.GetProductById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // ====================== DELETE (POST) ======================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (id == null) return NotFound();

            var product = _productService.GetProductById(id);
            if (product == null) return NotFound();

            try
            {
                var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "products", product.ProductId);
                if (Directory.Exists(uploadsRoot))
                {
                    Directory.Delete(uploadsRoot, true);
                }
            }
            catch { }

            _productService.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
