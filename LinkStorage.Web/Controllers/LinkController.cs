using LinkStorage.Business.Abscract;
using LinkStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkStorage.Web.Controllers
{
    [Authorize]
    public class LinkController : Controller
    {
        private readonly ILinkService _linkService;

        public LinkController(ILinkService linkService)
        {
            _linkService = linkService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAll()
        {
            var links = _linkService.GetAllLinks();
            return Json(new { data = links });
        }

        public IActionResult GetById(int id)
        {
            var link = _linkService.GetLinkById(id);
            return Ok(link);
        }

        [HttpPost]
        public IActionResult Add(Link link)
        {
            _linkService.Add(link);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(Link link)
        {
            _linkService.Update(link);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            bool isDeleted = _linkService.Delete(id);

            if (isDeleted)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error");
            }
        }
    }
}
