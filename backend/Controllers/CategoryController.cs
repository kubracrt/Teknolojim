using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]



    public class CategoryController : ControllerBase
    {
        private readonly eCommerceContext _context;

        public CategoryController(eCommerceContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            if (category == null)
                return BadRequest("Geçersiz veri");

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kategori Eklendi", category });
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory()
        {
            var categories = await _context.Categories.ToListAsync();
            if (categories == null || !categories.Any())
                return NotFound("Kategori Bulunamadı");
            return Ok(categories);

        }

        [HttpGet("{categoryName}")]
        public async Task<IActionResult> GetProductsCategory(string categoryName)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            if (category == null)
            {
                return NotFound("Kategori Bulunamadı");
            }

            var products = await _context.Products
                .Where(p => p.CategoryId == category.Id)
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound("Ürün Bulunamadı");
            }

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 32
            };

            return Ok(products);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return BadRequest("");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ürün Silindi", category });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category updateCategory)
        {
            if (id != updateCategory.Id)
            {
                return BadRequest("ID ler uyuşmuyor");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return BadRequest("Kategori Bulunamadı");
            }

            category.Name = updateCategory.Name;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Kategori Güncelleme Başarılı", category });
        }


    }
}