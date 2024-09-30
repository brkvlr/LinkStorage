using LinkStorage.Business.Abstract;
using LinkStorage.Business.Shared.Concrete;
using LinkStorage.Models;
using LinkStorage.Repository.Shared.Abstract;
using LinkStorage.Repository.Shared.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business.Concrete
{
    public class CategoryService : Service<Category>, ICategoryService
    {
        private readonly IRepository<Category> _repository;

        public CategoryService(IRepository<Category> repository) : base(repository)
        {
            _repository = repository;
        }

        public void AddCategory(Category category)
        {
            if (category != null)
            {
                _repository.Add(category);
            }
        }

        public void DeleteCategory(int id)
        {
            var category = _repository.GetFirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _repository.Delete(id);
            }
        }

        public IQueryable<Category> GetAllCategories()
        {
            return _repository.GetAll(); // Tüm kategorileri getir
        }

        public void UpdateCategory(Category category)
        {
            var existingCategory = _repository.GetById(category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name; // Gerekli alanlar güncellenir
                _repository.Save();
            }
        }
    }
}
