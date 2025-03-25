using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace backend.Models
{
    [Table("ShoppingCard")]
    public class ShoppingCard
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; } 

        public int Price {get;set;}

        public DateTime CreatedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } 
    }
}
