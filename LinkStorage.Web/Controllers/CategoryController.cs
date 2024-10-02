using LinkStorage.Business.Abstract;
using LinkStorage.Business.Concrete;
using LinkStorage.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkStorage.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece admin kullanıcılar erişebilsin
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var categories = _categoryService.GetAllCategories().ToList();
            return View(categories);
        }

        [HttpPost]
        public IActionResult Add(Category category)
        {
            if (ModelState.IsValid)
            {
                // Eğer Id 0'dan büyükse, güncelleme işlemi yapın
                if (category.Id > 0)
                {
                    _categoryService.UpdateCategory(category);
                }
                else
                {
                    _categoryService.AddCategory(category);
                }
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _categoryService.DeleteCategory(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryService.UpdateCategory(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }
    }
}
