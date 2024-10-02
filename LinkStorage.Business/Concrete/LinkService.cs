using LinkStorage.Business.Abscract;
using LinkStorage.Business.Abstract;
using LinkStorage.Business.Shared.Concrete;
using LinkStorage.Models;
using LinkStorage.Repository.Shared.Abstract;
using LinkStorage.Repository.Shared.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly ITagService _tagService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LinkService(IRepository<Link> repository, IHttpContextAccessor httpContextAccessor, IUserService userService, ITagService tagService) : base(repository)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _tagService = tagService;
        }

        public Link Add(Link link, List<int> tagIds)
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
            return _repository.GetAll()
                .Include(l => l.User)
                .Include(l => l.Category)
                .Include(l => l.Tags.Where(l => !l.IsDeleted))
                .Include(l => l.Comments); // Gerekli ilişkileri dahil edin
        }
        public void Update(Link link)
        {
            var existingLink = _repository.GetFirstOrDefault(x => x.Id == link.Id);
            if (existingLink != null)
            {
                // Değerlerin doğru geldiğini kontrol edin
                Debug.Assert(link.UserId != null, "UserId is null");
                Debug.Assert(link.CategoryId != null, "CategoryId is null");
                // Diğer kontroller...

                existingLink.Description = link.Description;
                existingLink.Url = link.Url;
                //existingLink.CategoryId = link.CategoryId; // Kategori ID'sini güncelle
                //existingLink.UserId = link.UserId; // Kullanıcı ID'sini güncelle
                //existingLink.Tags = link.Tags; // Etiketleri güncelle

                _repository.Update(existingLink);
            }
        }

        public void Delete(int linkId)
        {
            var link = _repository.GetFirstOrDefault(l => l.Id == linkId);

            if (link != null)
            {
                // Link ile ilişkili tagları kontrol et
                foreach (var tag in link.Tags.ToList())
                {
                    var isTagUsedElsewhere = _repository.GetAll()
                        .Any(l => l.Tags.Any(t => t.Id == tag.Id && !t.IsDeleted));

                    if (!isTagUsedElsewhere)
                    {
                        _tagService.Delete(tag.Id); // Tag'ı sil
                    }
                }

                _repository.Delete(link.Id); // Link'i sil
            }
        }
    }
}