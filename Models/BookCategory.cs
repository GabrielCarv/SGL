using System.ComponentModel.DataAnnotations;

namespace MVC_Library.Models
{
    public class BookCategory
    {

        [Key]
        public int BookId { get; set; }
        [Key]
        public int CategoryId { get; set; }

        public virtual Book Book { get; set; }

        public virtual Category Category { get; set; }
    }
}
