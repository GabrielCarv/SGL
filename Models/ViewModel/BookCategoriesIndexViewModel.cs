using System.Collections.Generic;

namespace MVC_Library.Models
{
    public class BookCategoriesIndexViewModel
    {

        public Book  Book { get; set; }

        public List<Category> Categories { get; set; }
    }
}
