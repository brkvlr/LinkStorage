using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Models
{
    public class Comment:BaseModel
    {
        public string Text { get; set; }
        public int LinkId { get; set; }
        public Link Link { get; set; }

        public int UserId { get; set; }
        public virtual AppUser User { get; set; }

        public int? ParentCommentId { get; set; }
        public virtual Comment ParentComment {  get; set; }

        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
