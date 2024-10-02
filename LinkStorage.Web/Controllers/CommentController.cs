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
            var comments = _commentService.GetAllComments().ToList();
            return View(comments);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _commentService.DeleteComment(id);
            return RedirectToAction("Index");
        }

    }
}
