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

        public CommentService(IRepository<Comment> repository, IHttpContextAccessor httpContextAccessor) : base(repository)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        // Yorum ekleme ve göreve ait tüm yorumları döndürme işlemi
        public IQueryable<Comment> AddComment(Comment comment)
        {
            comment.UserId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _repository.Add(comment);
            return _repository.GetAll(c => c.LinkId == comment.LinkId).Include(c => c.User);
        }

        public Comment GetCommentById(int id)
        {
            return _repository.GetById(id);
        }
    }
}
