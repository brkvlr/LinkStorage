using LinkStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business.Abscract
{
    public interface ILinkService
    {
       public IQueryable<Link> GetAllLinks();
       public Link GetLinkById(int id);
       public Link Add(Link link);
        public Link Update(Link link);
        public bool Delete(int id);
    }
}
