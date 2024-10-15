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

        public void AddTag(string tagName)
        {
            // Tag ekleme işlemi
            var tag = new Tag { Name = tagName };
            _repository.Add(tag);
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

        public List<Tag> GetTagsByIds(List<int> tagIds)
        {
            // Yalnızca ilgili ID'lere sahip tag'leri çekin
            return _repository.GetAll().Where(tag => tagIds.Contains(tag.Id)).ToList();
        }

        public List<Tag> CreateNewTags(List<string> tagNames)
        {
            var existingTags = _repository.GetAll().Where(t => tagNames.Contains(t.Name)).ToList();
            var newTagNames = tagNames.Except(existingTags.Select(t => t.Name)).ToList();

            foreach (var newTagName in newTagNames)
            {
                var newTag = new Tag { Name = newTagName };
                _repository.Add(newTag);
            }
            _repository.Save();

            return _repository.GetAll().Where(t => tagNames.Contains(t.Name)).ToList();
        }
    }
}