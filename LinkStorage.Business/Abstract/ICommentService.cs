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
        bool DeleteComment(int id);
        Comment GetCommentById(int id);
        Comment Add(Comment comment);

        IEnumerable<Comment> GetCommentsByLinkId(int linkId);
    }
}