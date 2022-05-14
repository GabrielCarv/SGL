using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC_Library.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        public int BookId { get; set; }
        public List<BookCategory> BookCategories { get; set; }


    }
}
