using Microsoft.EntityFrameworkCore;
using Entities;
using Context;
using Microsoft.AspNetCore.Mvc;

namespace Services
{
    public class ShoppingCardService
    {
        private readonly eCommerceContext _context;

        public ShoppingCardService(eCommerceContext context)
        {

            _context = context;
        }

        public async Task<List<ShoppingCardDto>> GetShoppingCardsAsync()
        {
            var shoppingCards = await _context.ShoppingCards
            .Include(sc => sc.User)
            .Include(sc => sc.Product)
            .Select(sc => new ShoppingCardDto
            {
                Id = sc.Id,
                UserName = sc.User.Username,
                Price = sc.Price,
                ProductName = sc.Product.Name,
                ProductId = sc.ProductId,
                ImageUrl = sc.ImageUrl,
                quantity = sc.quantity,
                UserId = sc.UserId,

            }).ToListAsync();

            if (shoppingCards.Count > 0)
            {
                return shoppingCards;
            }
            else
            {
                return null;
            }

        }

        public async Task<List<ShoppingCardDto>> GetShoppingCardAsync(int UserId)
        {
            var shoppingCard = await _context.ShoppingCards
            .Where(sc => sc.UserId == UserId)
            .Include(sc => sc.User)
            .Include(sc => sc.Product)
            .Select(sc => new ShoppingCardDto
            {
                Id = sc.Id,
                UserName = sc.User.Username,
                Price = sc.Price,
                ProductName = sc.Product.Name,
                ProductId = sc.ProductId,
                ImageUrl = sc.ImageUrl,
                quantity = sc.quantity,
                UserId = sc.UserId

            }).ToListAsync();

            if (shoppingCard != null)
            {
                return shoppingCard;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<ShoppingCard>> AddShoppingCardAsync(ShoppingCard shoppingCard)
        {
            if (shoppingCard == null)
                return null;

            _context.ShoppingCards.Add(shoppingCard);
            await _context.SaveChangesAsync();

            return new List<ShoppingCard> { shoppingCard };
        }

        public async Task<bool> DeleteShoppingCardAsync(int id)
        {
            var shoppingCard = await _context.ShoppingCards.FindAsync(id);
            if (shoppingCard == null)
                return false;

            _context.ShoppingCards.Remove(shoppingCard);
            await _context.SaveChangesAsync();
            return true;
        }

    }

}