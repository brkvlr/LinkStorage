using LinkStorage.Business.Shared.Abstract;
using LinkStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business.Abstract
{
    public interface ICategoryService: IService<Category>
    {
        void AddCategory(Category category);
        void DeleteCategory(int id);
        IQueryable<Category> GetAllCategories();
        void UpdateCategory(Category category);
    }
}
