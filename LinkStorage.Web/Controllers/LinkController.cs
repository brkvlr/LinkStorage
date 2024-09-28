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
            if (ModelState.IsValid)
            {
                var result = _linkService.AddLink(link);
                return Ok(result);
            }
            return BadRequest("Link verileri geçersiz.");
        }

        [HttpPost]
        public IActionResult Update(Link link)
        {
            if (ModelState.IsValid)
            {
                var result = _linkService.Update(link);
                return Ok(result);
            }
            return BadRequest("Link güncellenirken bir hata oluştu.");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var result = _linkService.Delete(id);
            return Ok(result);
        }
    }
}
