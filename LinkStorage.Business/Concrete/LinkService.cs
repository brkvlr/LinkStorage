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
        private readonly ICommentService _commentService;
        private readonly ICategoryService _categoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LinkService(IRepository<Link> repository, IHttpContextAccessor httpContextAccessor, IUserService userService, ITagService tagService, ICategoryService categoryService, ICommentService commentService) : base(repository)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _tagService = tagService;
            _commentService = commentService;
            _categoryService = categoryService; 
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

        public void Add(Link link)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            var category = _categoryService.GetById(link.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {link.CategoryId} not found");
            }

            var user = _userService.GetById(link.UserId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {link.UserId} not found");
            }

            link.Category = category;
            link.User = user;

            _repository.Add(link);
            _repository.Save();
        }

        // Link Güncelleme Yöntemi
        public void UpdateLink(Link link, List<int> tagIds)
        {
            _repository.Update(link);

            var existingLink = _repository.GetFirstOrDefault(l => l.Id == link.Id);
            if (existingLink != null)
            {
                existingLink.Description = link.Description;
                existingLink.Url = link.Url;

                // Etiketleri güncelle
                existingLink.Tags.Clear();
                foreach (var tagId in tagIds)
                {
                    var tag = _tagService.GetAllTags().FirstOrDefault(t => t.Id == tagId);
                    if (tag != null)
                    {
                        existingLink.Tags.Add(tag);
                    }
                }

                _repository.Update(existingLink);
                _repository.Save(); // Veritabanına kaydet
            }
        }

        // Link Silme Yöntemi
        public bool DeleteLink(int linkId, string userId)
        {
            var link = _repository.GetFirstOrDefault(l => l.Id == linkId);

            if (link == null)
            {
                return false; // Link bulunamadı
            }

            // Kullanıcının yetkisini kontrol et
            if (link.UserId.ToString() != userId && !_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                return false; // Yetkisiz işlem
            }

            _repository.Delete(link.Id);
            _repository.Save(); // Veritabanına işlemi kaydet
            return true; // Başarılı silme
        }

    public IQueryable<Link> GetAllLinks()
        {
            return _repository.GetAll()
                .Include(l => l.User) // Kullanıcıyı dahil et
                .Include(l => l.Category) // Kategoriyi dahil et
                .Include(l => l.Tags.Where(t => !t.IsDeleted)) // Silinmemiş etiketleri dahil et
                .Include(l => l.Comments) // Yorumları dahil et
                    .ThenInclude(c => c.User) // Yorumlardaki kullanıcıyı dahil et
                .Include(l => l.Comments) // Yorumları dahil et
                    .ThenInclude(c => c.Replies) // Alt yorumları dahil et
                        .ThenInclude(r => r.User) // Alt yorumlardaki kullanıcıları dahil et
                .OrderByDescending(link => link.DateCreated);
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

        public void AddComment(int linkId, Comment comment)
        {
            // Linki ID'sine göre al
            var link = _repository.GetFirstOrDefault(l => l.Id == linkId);

            if (link != null)
            {
                // Yorumun ilişkilendirilmiş olduğu linke eklenmesi
                comment.LinkId = linkId;
                comment.DateCreated = DateTime.Now;

                // Yorum ekleme işlemi
                _commentService.Add(comment); // CommentService üzerinden ekleme
            }
        }

        public void UpdateLinkWithTags(Link link, List<string> tagNames)
        {
            var existingLink = _repository.GetFirstOrDefault(l => l.Id == link.Id);
            if (existingLink != null)
            {
                existingLink.Url = link.Url;
                existingLink.Description = link.Description;
                existingLink.CategoryId = link.CategoryId;

                // Etiketleri güncelle
                existingLink.Tags.Clear();
                var updatedTags = _tagService.CreateNewTags(tagNames);
                foreach (var tag in updatedTags)
                {
                    existingLink.Tags.Add(tag);
                }

                _repository.Update(existingLink);
                _repository.Save();
            }
        }

        public void UpdateLink(Link link)
        {
            var existingLink = _repository.GetFirstOrDefault(l => l.Id == link.Id);
            if (existingLink != null)
            {
                existingLink.Url = link.Url;
                existingLink.Description = link.Description;
                existingLink.CategoryId = link.CategoryId;

                // Etiketleri güncelle
                existingLink.Tags.Clear();
                if (link.Tags != null)
                {
                    foreach (var tag in link.Tags)
                    {
                        existingLink.Tags.Add(tag);
                    }
                }

                _repository.Update(existingLink);
                _repository.Save();
            }
        }

        public IQueryable<Link> GetLinksByCategoryAndTags(int? categoryId, List<int> tagIds)
        {
            var links = _repository.GetAll();

            if (categoryId.HasValue)
            {
                links = links.Where(l => l.Category.Id == categoryId.Value);
            }

            if (tagIds != null && tagIds.Count > 0)
            {
                links = links.Where(l => l.Tags.Any(t => tagIds.Contains(t.Id)));
            }

            return links;
        }

        public List<Link> GetLinksByUserId(int userId)
        {
            return _repository.GetAll()
                .Include(l => l.Category) // Kategori bilgilerini dahil et
                .Include(l => l.Tags) // Etiket bilgilerini dahil et
                .Where(l => l.UserId == userId) // Kullanıcıya ait linkleri al
                .ToList();
        }

        public Link GetLinkWithTags(int linkId)
        {
            return _repository.GetById(linkId); // Tag'ları alır
        }
    }
}