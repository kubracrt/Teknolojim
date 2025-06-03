// using Entities;
// using Context;
// using Microsoft.EntityFrameworkCore;

// namespace Services
// {
//     public class OrderService
//     {
//         private readonly eCommerceContext _context;

//         public OrderService(eCommerceContext context)
//         {
//             _context = context;
//         }

//         public async Task<List<OrderDto>> GetOrdersAsync()
//         {
//             var orders = await _context.Orders
//                 .Include(o => o.User)
//                 .Include(o => o.Product)
//                 .Select(o => new OrderDto
//                 {
//                     Id = o.Id,
//                     UserName = o.User.Username,
//                     Price = o.Price,
//                     UserId = o.UserId,
//                     ProductName = o.Product.Name,
//                     ImageUrl = o.ImageUrl,
//                     Quantity = o.quantity,
//                     OrderNumber = o.OrderNumber,
//                 })
//                 .ToListAsync();

//             return orders;
//         }

//         public async Task<List<OrderDto>> GetOrderAsync(int userId)
//         {
//             var orders = await _context.Orders
//                 .Where(o => o.UserId == userId)
//                 .Include(o => o.User)
//                 .Include(o => o.Product)
//                 .Select(o => new OrderDto
//                 {
//                     Id = o.Id,
//                     UserName = o.User.Username,
//                     Price = o.Price,
//                     UserId = o.UserId,
//                     ProductName = o.Product.Name,
//                     ImageUrl = o.Product.ImageUrl,
//                     Quantity = o.quantity,
//                     OrderNumber = o.OrderNumber,
//                 })
//                 .ToListAsync();

//             return orders;
//         }

//         public async Task<List<Order>> SaveOrder(List<Order> orders)
//         {
//             using (var transaction = await _context.Database.BeginTransactionAsync())
//             {
//                 try
//                 {
//                     foreach (var cartItem in orders)
//                     {
//                         var product = await _context.Products
//                                                     .Where(p => p.Id == cartItem.ProductId)
//                                                     .FirstOrDefaultAsync();

//                         if (product == null)
//                         {
//                             await transaction.RollbackAsync();
//                             return null;
//                         }

//                         if (product.Stock < cartItem.quantity)
//                         {
//                             await transaction.RollbackAsync();
//                             return null;
//                         }

//                         product.Stock -= cartItem.quantity;
//                         _context.Products.Update(product);

//                         _context.Orders.Add(cartItem);
//                     }

//                     await _context.SaveChangesAsync();
//                     await transaction.CommitAsync();

//                     return orders;
//                 }
//                 catch (Exception ex)
//                 {
//                     await transaction.RollbackAsync();
//                     return null;
//                 }
//             }
//         }

//     }
// }
