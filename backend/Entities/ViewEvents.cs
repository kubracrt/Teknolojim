using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Table("ViewEvents")]
    public class ViewEvents
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; } = 0;
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; } = null;
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;



    }
}