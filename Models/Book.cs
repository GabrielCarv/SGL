using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC_Library.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        [Display(Name = "Publish Date")]
        public DateTime PublishDate { get; set; }
        [Required]
        [Display(Name = "Base Price")]//"{0:0.00}"
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.###}")]
        public decimal BasePrice { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Display (Name ="Publishing Company")]
        public int PublishingCompanyId { get; set; }

        public virtual PublishingCompany PublishingCompany { get; set; }
        
        public virtual List<BookCategory> BookCategories { get; set; }

    }
}
