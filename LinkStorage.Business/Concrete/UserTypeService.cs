using LinkStorage.Business.Abscract;
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
        public class UserTypeService : Service<AppUserType>, IUserTypeService
        {
            private readonly IRepository<AppUserType> _userTypeRepository;

            public UserTypeService(IRepository<AppUserType> userTypeRepository) : base(userTypeRepository)
            {
                _userTypeRepository = userTypeRepository;
            }

            public IQueryable<AppUserType> GetAllType()
            {
                return _userTypeRepository.GetAll(u => u.Id != 1);
            }
        }
    }