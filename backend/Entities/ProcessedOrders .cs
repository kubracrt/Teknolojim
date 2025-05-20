using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Table("ProcessedOrders")]

    public class ProcessedOrders
    {
        [Key]
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
       
    }
}