using LinkStorage.Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkStorage.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagController : Controller
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        public IActionResult Index()
        {
            var tags = _tagService.GetAllTags().ToList();
            return View(tags);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _tagService.DeleteTag(id);
            return RedirectToAction("Index");
        }
    }
}