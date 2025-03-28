using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using backend.Models;
using Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ShoppingCardController : ControllerBase
    {
        private readonly eCommerceContext _context;

        public ShoppingCardController(eCommerceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetShoppingCards()
        {
            var shoppingCards = await _context.ShoppingCards
            .Include(sc => sc.User)
            .Include(sc => sc.Product)
            .Select(sc => new
            {
                sc.Id,
                userName = sc.User.Username,
                sc.Price,
                productName = sc.Product.Name,
                sc.ProductId,
                sc.ImageUrl,
                sc.quantity,

            }).ToListAsync();

            if (shoppingCards.Count > 0)
            {
                return Ok(shoppingCards);
            }
            else
            {
                return BadRequest("");
            }
        }

        [HttpGet("{UserId}")]

        public async Task<IActionResult> GetShoppingCard(int UserId)
        {
            var shoppingCard = await _context.ShoppingCards
            .Where(sc => sc.UserId == UserId)
            .Include(sc => sc.User)
            .Include(sc => sc.Product)
            .Select(sc => new
            {
                sc.Id,
                userName = sc.User.Username,
                sc.Price,
                sc.UserId,
                productName = sc.Product.Name,
                sc.ProductId,
                sc.ImageUrl,
                sc.quantity,

            }).ToListAsync();

            if (shoppingCard != null)
            {
                return Ok(shoppingCard);

            }
            else
            {
                return BadRequest("Ürün Yok");
            }
        }

        [HttpPost]

        public async Task<IActionResult> PostShoppingCard([FromBody] ShoppingCard shoppingCard)
        {
            _context.ShoppingCards.Add(shoppingCard);
            await _context.SaveChangesAsync();
            return Ok(shoppingCard);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            var deleteProduct = await _context.ShoppingCards.FindAsync(Id);
            if (deleteProduct == null)
            {
                return NotFound("Kullanıcı Bulunamadı");
            }
            _context.ShoppingCards.Remove(deleteProduct);
            await _context.SaveChangesAsync();
            return Ok(deleteProduct);
        }
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] List<Order> orders)
        {
            foreach (var order in orders)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == order.ProductId);
                if (product == null)
                {
                    return NotFound("Ürün Bulunamadı");
                }

                if (product.Stock < order.quantity)
                {
                    return BadRequest($"Yetersiz stok: {product.Name} için stokta {product.Stock} adet bulunmaktadır.");
                }

                product.Stock -= order.quantity;
            }

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            await _context.SaveChangesAsync();

            return Ok(orders);
        }


    }
}

