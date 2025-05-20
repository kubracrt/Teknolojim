using Context;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class ViewEventService
    {
        private readonly eCommerceContext _context;

        public ViewEventService(eCommerceContext context)
        {
            _context = context;
        }

       public async Task LogViewEventAsync(string userId, int productId, string name,string imageUrl)
        {
            var entity = new ViewEvents
            {
                UserId = int.TryParse(userId, out int uid) ? uid : 0,
                ProductId = productId,
                ProductName = name,
                ImageUrl = imageUrl,
                ViewedAt = DateTime.UtcNow
            };

            _context.ViewEvents.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ViewEvents>> GetLast30MinViewsAsync()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-30);
            return await _context.ViewEvents
                .Where(e => e.ViewedAt >= cutoff)
                .ToListAsync();
        }
    }

}