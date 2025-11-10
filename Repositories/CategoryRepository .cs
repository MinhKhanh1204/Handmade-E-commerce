using BussinessObject;
using DataAccessObject;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MyStoreContext _context;

        public CategoryRepository(MyStoreContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public Category? GetCategoryById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryId == id);
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        public void DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public bool Exists(int id)
        {
            return _context.Categories.Any(c => c.CategoryId == id && c.Status != "Deleted");
        }

        public IEnumerable<Category> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllCategories();
            }

            var term = searchTerm.ToLower().Trim();

            return _context.Categories
                .Where(c => c.Status != "Deleted" &&
                           ((c.CategoryName != null && c.CategoryName.ToLower().Contains(term)) ||
                            (c.Description != null && c.Description.ToLower().Contains(term))))
                .OrderBy(c => c.CategoryName ?? "")
                .ToList();
        }
    }
}
