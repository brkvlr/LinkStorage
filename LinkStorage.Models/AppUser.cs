using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Models
{
    [Table("Users")]
    public class AppUser : BaseModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int UserTypeId { get; set; }
        public virtual AppUserType UserType { get; set; }
        public ICollection<Link> Links { get; set; } = [];
        public ICollection<Comment> Comments { get; set; }
    }
}