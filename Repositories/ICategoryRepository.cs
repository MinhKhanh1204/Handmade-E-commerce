using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
        IEnumerable<Category> GetCategories();
        Category? GetCategoryById(int id);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int id);
        bool Exists(int id);
        IEnumerable<Category> Search(string searchTerm);
        IEnumerable<Category> GetActiveCategories();
    }
}
