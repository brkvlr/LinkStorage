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
    }
}
