using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    [Table("Category")]
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required List<Product> Products { get; set; }

    }
}



