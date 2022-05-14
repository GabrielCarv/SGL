using System.ComponentModel.DataAnnotations;

namespace MVC_Library.Models
{
    public class Phone
    {
        [Key]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }

        public virtual Person Person { get; set; }

    }
}
