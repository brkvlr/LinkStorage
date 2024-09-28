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
    public class LinkService : Service<Link>, ILinkService
    {
        private readonly IRepository<Link> _repository;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LinkService(IRepository<Link> repository, IHttpContextAccessor httpContextAccessor, IUserService userService) : base(repository)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public Link AddLink(Link link)
        {
            // Kullanıcının kimliğini al
            link.UserId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            // Linki ekle
            _repository.Add(link);
            _repository.Save(); // Eğer Save metodu varsa
            return link;
        }

        public IQueryable<Link> GetAllLinks()
        {
            // Kullanıcı ID'sini al
            int userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return _repository.GetAll()
                .Where(x => x.UserId == userId).Include(x => x.Comments) // Yorumları dahil et
                .Select(x => new Link
                {
                    Id = x.Id,
                    Url = x.Url,
                    Description = x.Description,
                    DateCreated = x.DateCreated,
                    UserId = x.UserId,
                    Comments = x.Comments.ToList() // Yorumları listele
                });
        }

        public Link GetLinkById(int id)
        {
            return _repository.GetAll()
                .Include(x => x.Comments) // Yorumları dahil et
                .FirstOrDefault(x => x.Id == id);
        }

        public Link Update(Link link)
        {
            _repository.Update(link);
            _repository.Save(); // Eğer Save metodu varsa
            return link;
        }

        public bool Delete(int id)
        {
            return _repository.Delete(id);
        }
    }
}