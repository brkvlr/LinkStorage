using LinkStorage.Business.Abscract;
using LinkStorage.Business.Shared.Concrete;
using LinkStorage.Models;
using LinkStorage.Repository.Shared.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business.Concrete
{
    public class CommentService : Service<Comment>, ICommentService
    {
        private readonly IRepository<Comment> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public CommentService(IRepository<Comment> repository, IHttpContextAccessor httpContextAccessor, IUserService userService) : base(repository)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public IEnumerable<Comment> GetAllComments()
        {
            return _repository.GetAll(); // Tüm yorumları getir
        }

        //public void DeleteComment(int id)
        //{
        //    _repository.Delete(id); // ID'yi direkt olarak gönderiyoruz
        //}

        public Comment Add(Comment comment)
        {
            try
            {
                _repository.Add(comment);
                _repository.Save();
                return comment;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error adding comment: {ex.Message}");
                return null;
            }
        }

        public IEnumerable<Comment> GetCommentsByLinkId(int linkId)
        {
            return _repository.GetAll()
                .Where(c => c.LinkId == linkId && c.ParentCommentId == null && !c.IsDeleted)
                .Include(c => c.User)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .OrderByDescending(c => c.DateCreated)
                .ToList();
        }

        public Comment GetCommentById(int id)
        {
            return _repository.GetAll()
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id == id);
        }

        public bool DeleteComment(int id)
        {
            try
            {
                var comment = _repository.GetById(id);
                if (comment != null)
                {
                    DeleteReplies(comment);
                    _repository.Delete(id);
                    _repository.Save();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error deleting comment: {ex.Message}");
                return false;
            }
        }

        private void DeleteReplies(Comment comment)
        {
            var replies = _repository.GetAll().Where(c => c.ParentCommentId == comment.Id).ToList();
            foreach (var reply in replies)
            {
                DeleteReplies(reply);
                _repository.Delete(reply.Id);
            }
        }
    }
}
