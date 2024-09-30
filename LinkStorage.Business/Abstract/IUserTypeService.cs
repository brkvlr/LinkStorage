using LinkStorage.Business.Shared.Abstract;
using LinkStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business.Abscract
{
    public interface IUserTypeService: IService<AppUserType>
    {
        IQueryable<AppUserType> GetAllType();
    }
}
