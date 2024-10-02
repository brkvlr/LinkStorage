using LinkStorage.Business.Shared.Abstract;
using LinkStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business.Abstract
{
    public interface ITagService : IService<Tag>
    {
        void AddTag(Tag tag);
        IEnumerable<Tag> GetAllTags();
        void DeleteTag(int id);
    }
}
