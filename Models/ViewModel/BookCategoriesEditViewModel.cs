using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Library.Models
{
    public class BookCategoriesEditViewModel
    {
        public Book Book { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public IEnumerable<int> SelectedCategories { get; set; }


    }
}
