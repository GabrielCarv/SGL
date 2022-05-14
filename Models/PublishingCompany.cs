using System.ComponentModel.DataAnnotations;

namespace MVC_Library.Models
{
    public class PublishingCompany
    {
        [Key]
        public int PublishingCompanyId { get; set; }

        [Required]
        [Display(Name = "Publishing Company Name")]
        public string PublishingCompanyName { get; set; }
    }
}
