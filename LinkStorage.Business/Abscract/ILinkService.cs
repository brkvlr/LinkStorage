using LinkStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business.Abscract
{
    internal interface ILinkService
    {
        IQueryable<Link> GetAllLinks();
        Link GetLinkById(int id);
        Link AddLink(Link link);
        Link Update(Link link);
        bool Delete(int id);
    }
}
