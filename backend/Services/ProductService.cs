using Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Entities;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Npgsql;

namespace Services
{
    public class ProductService
    {
        private readonly eCommerceContext _context;

        private readonly IDatabase _redisDatabase;

        private readonly string _redisKey = "TopProducts";

        public ProductService(eCommerceContext context, IDatabase redisDatabase)
        {
            _context = context;
            _redisDatabase = redisDatabase;
        }


        public async Task<List<ProductDto>> GetProductsAsync()
        {
            // var products = await _context.Products
            // .Include(p => p.Category)
            // .Include(p => p.User)
            // .Select(p => new ProductDto
            // {
            //     Id = p.Id,
            //     UserName = p.User.Username,
            //     Name = p.Name,
            //     Price = p.Price,
            //     ImageUrl = p.ImageUrl,
            //     Stock = p.Stock,
            //     CategoryName = p.Category.Name


            // }).ToListAsync();

            var products = await _context.ProductDtos
                .FromSqlRaw("Select * from get_all_products()")
                .ToListAsync();

            return products;


        }

        public async Task<ProductDto> GetProductAsync(int Id)
        {
            var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.User)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                UserName = p.User.Username,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Stock = p.Stock,
                CategoryName = p.Category.Name


            }).FirstOrDefaultAsync(p => p.Id == Id);
            if (product == null)
            {
                return null;
            }
            return product;
        }



        public async Task<List<ProductDto>> GetProductAdminAsync(int UserId)
        {
            var adminProducts = await _context.ProductDtos
                 .FromSqlRaw("SELECT * FROM get_products_by_admin({0})", UserId)
                 .ToListAsync();

            return adminProducts;
        }

        public async Task<Product> AddProductAsync([FromBody] Product product)
        {
            // var lastProduct = await _context.Products.OrderByDescending(p => p.Id).FirstOrDefaultAsync();
            // if (lastProduct == null)
            // {
            //     product.Id = 1;
            // }
            // else
            // {
            //     product.Id = lastProduct.Id + 1;
            // }

            // _context.Products.Add(product);
            // await _context.SaveChangesAsync();
            // return lastProduct;


            var sql = "SELECT add_product(@name, @price, @image_url, @stock,@categoryId, @userId)";
            var parameters = new[]
            {
                    new NpgsqlParameter("@name", product.Name),
                    new NpgsqlParameter("@price", product.Price),
                    new NpgsqlParameter("@image_url", product.ImageUrl),
                    new NpgsqlParameter("@stock", product.Stock),
                    new NpgsqlParameter("@categoryId", product.CategoryId),
                    new NpgsqlParameter("@userId", product.UserId)
                };

            await _context.Database.ExecuteSqlRawAsync(sql, parameters);

            return product;

        }

        public async Task<Product> DeleteProductAsync(int id)
        {
            // var product = await _context.Products.FindAsync(id);
            // if (product == null)
            // {
            //     return null;
            // }
            // _context.Products.Remove(product);
            // await _context.SaveChangesAsync();
            // return product;

            await _context.Database.ExecuteSqlRawAsync("SELECT delete_product({0})", id);
            return null;
        }

        public async Task<Product> UpdateProductAsync(int id, Product updateProduct)
        {
            // if (id != updateProduct.Id)
            // {
            //     return null;
            // }

            // var product = await _context.Products.FindAsync(id);
            // if (product == null)
            // {
            //     return null;
            // }

            // product.Name = updateProduct.Name;
            // product.Price = updateProduct.Price;
            // product.CategoryId = updateProduct.CategoryId;
            // product.ImageUrl = updateProduct.ImageUrl;
            // product.Stock = updateProduct.Stock;

            // await _context.SaveChangesAsync();
            // return product;

            var sql = "SELECT update_product({0}, {1}, {2}, {3}, {4}, {5})";
            await _context.Database.ExecuteSqlRawAsync(sql,
              id,
              updateProduct.Name,
              updateProduct.Price,
              updateProduct.ImageUrl,
              updateProduct.Stock,
              updateProduct.CategoryId
            );
            return updateProduct;
        }

    }
}




















