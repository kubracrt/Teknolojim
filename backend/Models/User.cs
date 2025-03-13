using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models

{

    [Table("User")]
    public class User
    {
        public int Id {get;set;}
        public string Username { get; set; }
        public string Password { get; set; }
        public Boolean isAdmin { get; set; }

    }
}