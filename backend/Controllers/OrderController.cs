using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using Npgsql;
using Prometheus;

namespace backend.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]

    public class OrderController : ControllerBase
    {
        private readonly eCommerceContext _context;
        private readonly KafkaProducerService _kafkaProducerService;
        private static readonly Counter OrderCreatedCounter = Metrics
            .CreateCounter("orders_created_total", "Total number of orders created.");

        public OrderController(eCommerceContext context, IConfiguration config)
        {
            _context = context;
            _kafkaProducerService = new KafkaProducerService(config);
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
                email = o.Usermail,

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
                email = o.Usermail,


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
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var orderNumber = Guid.NewGuid().ToString();
            var createdDate = DateTime.UtcNow;

            foreach (var order in orders)
            {
                var sql = "CALL create_order_procedure(@user_id, @product_id, @quantity, @imageurl, @ordernumber, @orderdate)";
                var parameters = new[]
            {

            new NpgsqlParameter("@user_id", order.UserId),
            new NpgsqlParameter("@product_id", order.ProductId),
            new NpgsqlParameter("@quantity", order.quantity),
            new NpgsqlParameter("@imageurl", order.ImageUrl),
            new NpgsqlParameter("@ordernumber", orderNumber),
            new NpgsqlParameter("@orderdate", createdDate)
        };

                await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == order.UserId);
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == order.ProductId);

                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    UserName = user?.Username ?? "Unknown",
                    Usermail = order.Usermail,
                    ProductId = order.ProductId,
                    ImageUrl = order.ImageUrl,
                    Price = product?.Price ?? 0,
                    quantity = order.quantity,
                    OrderNumber = orderNumber,
                    CreatedDate = createdDate
                };

                await _kafkaProducerService.SendOrderAsync(orderDto);

                // await _context.Orders.AddAsync(order);
                // await _context.SaveChangesAsync();

                OrderCreatedCounter.Inc();

            }

            scope.Complete();
            return Ok(new { message = "Siparişler başarıyla oluşturuldu" });
        }

    }
}


