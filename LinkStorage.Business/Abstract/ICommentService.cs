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
        IEnumerable<Comment> GetAllComments();
        void DeleteComment(int id);
        IQueryable<Comment> AddComment(Comment comment);
    }
}
