using LinkStorage.Business.Shared.Abstract;
using LinkStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Business.Abscract
{
    public interface ICommentService : IService<Comment>
    {
        IQueryable<Comment> AddComment(Comment comment);
    }
}
