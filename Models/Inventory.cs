using System.ComponentModel.DataAnnotations;

namespace MVC_Library.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        public string Note { get; set; }
        [Required]
        [Display(Name = "Damaged")]
        public bool IsDamaged { get; set; }

        [Required]
        public int BookId { get; set; }

        public virtual Book Book { get; set; }

    }
}

