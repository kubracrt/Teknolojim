using Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductController : ControllerBase

    {
        private readonly eCommerceContext _context;

        public ProductController(eCommerceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.User)
            .Select(p => new
            {
                p.Id,
                UserName = p.User.Username,
                p.Name,
                p.Price,
                p.ImageUrl,
                p.Stock,
                CategoryName = p.Category.Name


            }).ToListAsync();


            if (products.Count > 0)
            {
                return Ok(products);
            }
            else
            {
                return BadRequest("");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Ürün Bulunamadı");
            }
            return Ok(product);
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetProductAdmin(int UserId)
        {
            var adminProducts = await _context.Products
                .Where(p => p.UserId == UserId)  
                .ToListAsync();  

            if (adminProducts == null || !adminProducts.Any())
            {
                return NotFound("Ürün Bulunamadı");
            }

            return Ok(adminProducts); 
        }


        [HttpPost]
        public async Task<IActionResult> AddProducts([FromBody] Product product)
        {
            var lastProduct = await _context.Products.OrderByDescending(p => p.Id).FirstOrDefaultAsync();
            if (lastProduct == null)
            {
                product.Id = 1;
            }
            else
            {
                product.Id = lastProduct.Id + 1;

            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
        }



        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Ürün Bulunamadı");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ürün Silindi", product });


        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product updateProduct)
        {
            if (id != updateProduct.Id)
            {
                return BadRequest("ID ler uyuşmuyor");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return BadRequest("Ürün Bulunamadı");
            }

            product.Name = updateProduct.Name;
            product.Price = updateProduct.Price;
            product.CategoryId = updateProduct.CategoryId;
            product.ImageUrl = updateProduct.ImageUrl;
            product.Stock=updateProduct.Stock;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Ürün güncelleme başarılı", product });
        }
    }
}

