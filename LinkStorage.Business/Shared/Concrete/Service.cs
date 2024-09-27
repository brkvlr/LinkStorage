using LinkStorage.Business.Shared.Abstract;
using LinkStorage.Models;
using LinkStorage.Repository.Shared.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business.Shared.Concrete
{
    public class Service<T> : IService<T> where T : BaseModel
    {
        private readonly IRepository<T> _repo;

        public Service(IRepository<T> repo)
        {
            _repo = repo;
        }

        public T Add(T entity)
        {
            return _repo.Add(entity);
        }

        public bool Delete(int id)
        {
            return (_repo.Delete(id));
        }

        public IQueryable<T> GetAll()
        {
            return _repo.GetAll();
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _repo.GetAll(predicate);
        }

        public T GetByGuid(Guid guid)
        {
            return _repo.GetByGuid(guid);
        }

        public T GetById(int id)
        {
            return _repo.GetById(id);
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _repo.GetFirstOrDefault(predicate);
        }

        public bool HardDelete(int id)
        {
            return _repo.HardDelete(id);
        }

        public T Update(T entity)
        {
            return _repo.Update(entity);
        }
    }
}