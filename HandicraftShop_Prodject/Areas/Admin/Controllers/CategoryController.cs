using Microsoft.AspNetCore.Mvc;
using Services;
using BussinessObject;

namespace HandicraftShop_Prodject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Admin/Category
        public IActionResult Index(string searchTerm)
        {
            IEnumerable<Category> categories;
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                categories = _categoryService.SearchCategories(searchTerm);
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                categories = _categoryService.GetAllCategories();
            }
            
            return View(categories);
        }

        // GET: Admin/Category/Details/5
        public IActionResult Details(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CategoryName,Description,Status")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (_categoryService.CreateCategory(category))
                {
                    TempData["Success"] = "Category created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Failed to create category. Please try again.";
                }
            }
            return View(category);
        }

        // GET: Admin/Category/Edit/5
        public IActionResult Edit(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("CategoryId,CategoryName,Description,Status")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (_categoryService.UpdateCategory(category))
                {
                    TempData["Success"] = "Category updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Failed to update category. Please try again.";
                }
            }
            return View(category);
        }

        // GET: Admin/Category/Delete/5
        public IActionResult Delete(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_categoryService.DeleteCategory(id))
            {
                TempData["Success"] = "Category deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete category. Please try again.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
