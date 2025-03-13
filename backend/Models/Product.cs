using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Models
{

    [Table("Product")]
    public class Product
    {

        public int Id { get; set; }
        public required string Name { get; set; }
        public float Price { get; set; }
      
        public string ImageUrl { get; set; } = "";

        public int CategoryId { get; set; }

        public int Stock {get;set;}

        [JsonIgnore] 

        public Category? Category { get; set; }

    }
}