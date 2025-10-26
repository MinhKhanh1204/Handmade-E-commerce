using BussinessObject;
using System.Collections.Generic;

namespace Services
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategories();
        Category? GetCategoryById(int id);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int id);
    }
}
