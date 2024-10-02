using LinkStorage.Business.Abstract;
using LinkStorage.Business.Shared.Concrete;
using LinkStorage.Models;
using LinkStorage.Repository.Shared.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LinkStorage.Business.Concrete
{
    public class TagService : Service<Tag>, ITagService
    {
        private readonly IRepository<Tag> _repository;

        public TagService(IRepository<Tag> repository) : base(repository)
        {
            _repository = repository;
        }

        public void AddTag(Tag tag)
        {
            _repository.Add(tag);
            _repository.Save();
        }

        public IEnumerable<Tag> GetAllTags()
        {
            return _repository.GetAll();
        }

        public void DeleteTag(int id)
        {
            var tag = _repository.GetFirstOrDefault(t => t.Id == id);
            if (tag != null)
            {
                _repository.Delete(tag.Id);
                _repository.Save();
            }
        }
    }
}