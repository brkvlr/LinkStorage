using LinkStorage.Business.Abscract;
using LinkStorage.Business.Abstract;
using LinkStorage.Business.Concrete;
using LinkStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkStorage.Web.Controllers
{
    [Authorize]
    public class LinkController : Controller
    {
        private readonly ILinkService _linkService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;

        public LinkController(ILinkService linkService, ICategoryService categoryService, ITagService tagService)
        {
            _linkService = linkService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        public IActionResult Index()
        {
            var links = _linkService.GetAllLinks().ToList();
            var categories = _categoryService.GetAllCategories().ToList();

            ViewBag.Categories = categories; 

            return View(links);
        }

        public IActionResult GetAll()
        {
            var links = _linkService.GetAllLinks();
            return Json(new { data = links });
        }

        public IActionResult GetById(int id)
        {
            var link = _linkService.GetById(id);
            return Ok(link);
        }

        [HttpPost]
        public IActionResult Add(Link link)
        {
            _linkService.Add(link);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var link = _linkService.GetById(id); // Linki ID'sine göre al
            if (link == null)
            {
                return NotFound(); // Link bulunamazsa hata döndür
            }

            ViewBag.Categories = _categoryService.GetAllCategories(); // Kategorileri görünüm için ayarlayın

            // Tags'ları bir string olarak virgülle ayırarak gönderebilirsiniz
            ViewBag.Tags = string.Join(", ", link.Tags.Select(t => t.Name)); // Tags'ı al
            return View(link); // Link modelini görünümde gönder
        }

        [HttpPost]
        public IActionResult Update(Link link)
        {
            if (ModelState.IsValid)
            {
                try
                {

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the link: " + ex.Message);
                    Console.WriteLine(ex);
                }
            }

            ViewBag.Categories = _categoryService.GetAllCategories();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _linkService.Delete(id);
                _tagService.DeleteTag(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}
