using LinkStorage.Business.Abscract;
using LinkStorage.Business.Concrete;
using LinkStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkStorage.Web.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;

        public CommentController(ICommentService commentService, IUserService userService)
        {
            _commentService = commentService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var comments = _commentService.GetAllComments().ToList();
            return View(comments);
        }


        [HttpPost]
        [AllowAnonymous]
        public IActionResult Add(int linkId, string text, int? parentCommentId)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest("Yorum metni boş olamaz.");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var comment = new Comment
            {
                Text = text,
                LinkId = linkId,
                UserId = userId,
                ParentCommentId = parentCommentId,
                DateCreated = DateTime.Now
            };

            var addedComment = _commentService.Add(comment);

            if (addedComment == null)
            {
                return StatusCode(500, "Yorum eklenirken bir hata oluştu.");
            }

            addedComment.User = _userService.GetById(userId);

            return PartialView("_CommentPartial", addedComment);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var comment = _commentService.GetCommentById(id);
            if (comment == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (comment.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var result = _commentService.DeleteComment(id);
            return result ? Ok() : StatusCode(500, "Yorum silinirken bir hata oluştu.");
        }


        [HttpGet]
        public IActionResult GetComments(int linkId)
        {
            var comments = _commentService.GetCommentsByLinkId(linkId);
            return PartialView("_CommentList", comments);
        }


    }
}
