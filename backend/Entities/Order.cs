using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities;

namespace Entities
{

    [Table("Orders")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public decimal Price { get; set; }

        public int UserId { get; set; }

        public string Usermail { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; } = null;
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
        public int quantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string OrderNumber { get; set; } = Guid.NewGuid().ToString();

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    }
}