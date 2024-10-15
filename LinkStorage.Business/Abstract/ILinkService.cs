using LinkStorage.Business.Shared.Abstract;
using LinkStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business.Abscract
{
    public interface ILinkService : IService<Link>
    {
        IQueryable<Link> GetAllLinks();
        Link Add(Link link);
        Link Update(Link link);
        bool Delete(int id);
        bool DeleteLink(int linkId, string currentUserId);
        void AddComment(int linkId, Comment comment);
        IQueryable<Link> GetLinksByCategoryAndTags(int? categoryId, List<int> tagIds);
        List<Link> GetLinksByUserId(int userId);
        void UpdateLink(Link link);
        Link GetLinkWithTags(int linkId);
        void UpdateLink(Link link, List<int> tagIds);
        void UpdateLinkWithTags(Link link, List<string> tagNames);
    }
}
