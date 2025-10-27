using BussinessObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return _categoryRepository.GetAll();
        }

        public Category GetCategoryById(int id)
        {
            return _categoryRepository.GetById(id);
        }

        public bool CreateCategory(Category category)
        {
            try
            {
                _categoryRepository.Add(category);
                _categoryRepository.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateCategory(Category category)
        {
            try
            {
                if (!_categoryRepository.Exists(category.CategoryId))
                    return false;

                _categoryRepository.Update(category);
                _categoryRepository.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteCategory(int id)
        {
            try
            {
                if (!_categoryRepository.Exists(id))
                    return false;

                _categoryRepository.Delete(id);
                _categoryRepository.SaveChanges();
                return true;
            }
            catch
            {
                return false;
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
