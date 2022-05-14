using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_Library.Models
{
    public class Rent
    {
        [Key]
        public int RentId { get; set; }

        [Required]
        public DateTime RentDate { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; }

        public DateTime RealReturnDate { get; set; }

        [Required]
        public string Cpf { get; set; }

        public virtual Person Person { get; set; }

        [Required]
        public int InventoryId { get; set; }

        public virtual Inventory Inventory { get; set; }

    }
}
