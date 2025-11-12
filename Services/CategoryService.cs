using BussinessObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using DataAccessObject;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public IEnumerable<Category> GetCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public IEnumerable<Category> GetActiveCategories()
        {
            return _categoryRepository.GetActiveCategories();
        }

        public Category? GetCategoryById(int id)
        {
            return _categoryRepository.GetCategoryById(id);
        }

        public void AddCategory(Category category)
        {
            _categoryRepository.AddCategory(category);
        }

        public void UpdateCategory(Category category)
        {
            _categoryRepository.UpdateCategory(category);
        }

        public void DeleteCategory(int id)
        {
            var cat = _categoryRepository.GetCategoryById(id);
            if (cat != null)
            {
                _categoryRepository.DeleteCategory(id);
            }
        }

        public bool CategoryExists(int id)
        {
            return _categoryRepository.Exists(id);
        }

        public IEnumerable<Category> SearchCategories(string searchTerm)
        {
            return _categoryRepository.Search(searchTerm);
        }
    }
}
