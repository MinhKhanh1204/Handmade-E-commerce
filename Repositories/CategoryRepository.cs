using BussinessObject;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MyStoreContext _context;

        public CategoryRepository(MyStoreContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetAll()
        {
            return _context.Categories
                .Where(c => c.Status != "Deleted")
                .OrderBy(c => c.CategoryName)
                .ToList();
        }

        public Category GetById(int id)
        {
            return _context.Categories
                .FirstOrDefault(c => c.CategoryId == id && c.Status != "Deleted");
        }

        public void Add(Category category)
        {
            category.Status = "Active";
            _context.Categories.Add(category);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void Delete(int id)
        {
            var category = GetById(id);
            if (category != null)
            {
                category.Status = "Deleted";
                _context.Categories.Update(category);
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public bool Exists(int id)
        {
            return _context.Categories.Any(c => c.CategoryId == id && c.Status != "Deleted");
        }

        public IEnumerable<Category> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAll();
            }

            var term = searchTerm.ToLower().Trim();
            
            return _context.Categories
                .Where(c => c.Status != "Deleted" && 
                           (c.CategoryName.ToLower().Contains(term) || 
                            (c.Description != null && c.Description.ToLower().Contains(term))))
                .OrderBy(c => c.CategoryName)
                .ToList();
        }
    }
}
