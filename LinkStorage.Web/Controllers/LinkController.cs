using LinkStorage.Business.Abscract;
using LinkStorage.Business.Abstract;
using LinkStorage.Business.Concrete;
using LinkStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LinkStorage.Web.Controllers
{
    [Authorize]
    public class LinkController : Controller
    {
        private readonly ILinkService _linkService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LinkController(ILinkService linkService, ICategoryService categoryService, ITagService tagService, IHttpContextAccessor httpContextAccessor)
        {
            _linkService = linkService;
            _categoryService = categoryService;
            _httpContextAccessor = httpContextAccessor; 
            _tagService = tagService;
        }

        public IActionResult Index()
        {
            var links = _linkService.GetAllLinks().ToList();
            var categories = _categoryService.GetAllCategories().ToList();

            ViewBag.Categories = categories; 

            return View(links.ToList());
        }


        public IActionResult MyLinks()
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var categories = _categoryService.GetAllCategories(); 

            ViewBag.Categories = categories; 

            var links = _linkService.GetLinksByUserId(userId);
            return View(links);
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            var links = _linkService.GetAllLinks()
                .Where(l => l.Url.Contains(query) || l.Description.Contains(query))
                .ToList();
            return View("Feed", links);
        }

        [HttpPost]
        public IActionResult AddLink(string url, string description, int categoryId, string tags)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(description) || categoryId <= 0)
            {
                return BadRequest("Geçersiz link bilgileri.");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var link = new Link
            {
                Url = url,
                Description = description,
                CategoryId = categoryId,
                UserId = userId,
                DateCreated = DateTime.Now
            };

            if (!string.IsNullOrEmpty(tags))
            {
                var tagList = tags.Split(',').Select(t => t.Trim()).ToList();
                var createdTags = _tagService.CreateNewTags(tagList);
                link.Tags = createdTags;
            }

            _linkService.Add(link);

            return Ok(new { message = "Link başarıyla eklendi." });
        }

        [HttpGet]
        public IActionResult GetLink(int id)
        {
            var link = _linkService.GetLinkWithTags(id);
            if (link == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = link.Id,
                url = link.Url,
                description = link.Description,
                categoryId = link.CategoryId,
                tags = link.Tags.Select(t => t.Name).ToList() 
            });
        }


    [HttpPost]
        public IActionResult UpdateLink(int id, string url, string description, int categoryId, string tags)
        {
            var link = _linkService.GetLinkWithTags(id);
            if (link == null)
            {
                return NotFound();
            }

            link.Url = url;
            link.Description = description;
            link.CategoryId = categoryId;

            // Etiket güncellemesi
            var tagList = string.IsNullOrWhiteSpace(tags) ? new List<string>() : tags.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
            _linkService.UpdateLinkWithTags(link, tagList);

            return Ok(new { message = "Link başarıyla güncellendi." });
        }


        // bu kullanıcılar için Link Silme
        [HttpPost]
        public IActionResult DeleteLink(int id)
        {

            return Ok(_linkService.Delete(id));
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


        // bu admin panelinden user kontrol
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _linkService.Delete(id);
                _tagService.DeleteTag(id);
                return RedirectToAction("Feed");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public IActionResult Feed()
        {
            ViewBag.Categories = _categoryService.GetAllCategories();
            ViewBag.Tags = _tagService.GetAllTags();

            var links = _linkService.GetAllLinks();
            return View(links);
        }

        [HttpGet]
        public IActionResult FilterLinks(string searchQuery, int? categoryId, [FromQuery] List<int> tagIds)
        {
            var links = _linkService.GetAllLinks();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                links = links.Where(l => l.Url.Contains(searchQuery) || l.Description.Contains(searchQuery));
            }

            if (categoryId.HasValue && categoryId.Value != 0)
            {
                links = links.Where(l => l.CategoryId == categoryId.Value);
            }

            if (tagIds != null && tagIds.Any())
            {
                links = links.Where(l => l.Tags.Any(t => tagIds.Contains(t.Id)));
            }

            links = links.OrderByDescending(l => l.DateCreated);

            var model = links.ToList();

            return PartialView("_LinkList", model);
        }

    }
}
