using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Models
{
    [Table("Categories")]
    public class Category : BaseModel
    {
        [Required(ErrorMessage = "Kategori adı boş olamaz.")]
        public string Name { get; set; }
        public ICollection<Link> Links { get; set; }
    }
}
