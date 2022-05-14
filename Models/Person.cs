using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace MVC_Library.Models
{
    public class Person
    {
        [Key]
        [StringLength(11, MinimumLength = 11)]
        public string Cpf{ get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Required]
        [Display(Name = "Librarian")]
        public bool IsLibrarian { get; set; }


        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        public string CEP{ get; set; }

        [Required]
        public string State { get; set; }
        [Required]
        public string City  { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [Display(Name = "House Number")]
        public int HouseNumber { get; set; }

        public virtual ICollection<Phone> Phones{ get; set; }
        
    }
}

