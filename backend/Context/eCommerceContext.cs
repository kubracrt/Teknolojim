using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Context
{
    public class eCommerceContext :DbContext
    {
        public DbSet<Category> Categories {get;set;}
        public DbSet<Product> Products {get;set;}

        public DbSet<User>Users{get;set;}

        public eCommerceContext(DbContextOptions<eCommerceContext> options) : base(options) {}

    }
}