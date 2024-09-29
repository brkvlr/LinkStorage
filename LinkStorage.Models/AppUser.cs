using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Models
{
    [Table("Users")]
    public class AppUser : BaseModel
    {
        [Required(ErrorMessage = "Kullanıcı Adı gereklidir.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi girin.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Şifre gereklidir.")]
        [MinLength(3, ErrorMessage = "Şifre en az 3 karakter olmalıdır.")]
        public string? Password { get; set; }
        public int UserTypeId { get; set; }
        public virtual AppUserType? UserType { get; set; }
        public ICollection<Link> Links { get; set; } = [];
        public ICollection<Comment>? Comments { get; set; }
    }
}