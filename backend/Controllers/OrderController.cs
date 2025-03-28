using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]

    public class OrderController : ControllerBase
    {
        private readonly eCommerceContext _context;

        public OrderController(eCommerceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Product)
            .Select(o => new
            {
                o.Id,
                userName = o.User.Username,
                o.Price,
                o.UserId,
                productName = o.Product.Name,
                o.ImageUrl,
                o.quantity,
                o.OrderNumber,

            }).ToListAsync();

            if (orders.Count > 0)
            {
                return Ok(orders);
            }
            else
            {
                return BadRequest("Sipariş Bulunamadı");
            }
        }

        [HttpGet("{UserId}")]

        public async Task<IActionResult> GetOrder(int UserId)
        {
            var order = await _context.Orders
            .Where(o => o.UserId == UserId)
            .Include(o => o.User)
            .Include(o => o.Product)
            .Select(o => new
            {
                o.Id,
                userName = o.User.Username,
                o.Price,
                o.UserId,
                productName = o.Product.Name,
                o.ImageUrl,
                o.quantity,
                o.OrderNumber,
            }).ToListAsync();

            if (order != null)
            {
                return Ok(order);
            }
            else
            {
                return BadRequest("Sipariş Yok");
            }
        }




        [HttpPost]
        public async Task<IActionResult> SaveOrder([FromBody] List<Order> orders)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var cartItem in orders)
                    {
                        var product = await _context.Products
                                                    .Where(p => p.Id == cartItem.ProductId)
                                                    .FirstOrDefaultAsync();

                        if (product == null)
                        {
                            await transaction.RollbackAsync();
                            return NotFound($"Ürün Bulunamadı: {cartItem.ProductId}");
                        }

                        if (product.Stock < cartItem.quantity)
                        {
                            await transaction.RollbackAsync();
                            return BadRequest("Yetersiz Stok");
                        }

                        product.Stock -= cartItem.quantity;
                        _context.Products.Update(product);

                        _context.Orders.Add(cartItem);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Sipariş Tamamlandı");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Sunucu Hatası: " + ex.Message);
                }
            }
        }


    }
}


