using LinkStorage.Business.Abscract;
using LinkStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkStorage.Web.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add([FromBody] Comment comment)
        {
            if (comment == null)
            {
                return BadRequest("Yorum satırı boş lütfen yorum yazınız.");
            }

            var comments = _commentService.AddComment(comment); // Yorum eklendikten sonra aynı linke ait tüm yorumlar
            return Ok(comments);
        }

        // Belirli bir yorumu ve onun alt yorumlarını getirir
        [HttpGet]
        public IActionResult GetCommentById(int id)
        {
            var comment = _commentService.GetCommentById(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }
    }
}
