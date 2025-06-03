using StackExchange.Redis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Context;
using Entities;

namespace Services
{
    public class RedisProductService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly eCommerceContext _context;

        public RedisProductService(IConnectionMultiplexer redis, eCommerceContext context)
        {
            _redis = redis;
            _context = context;
        }
        public async Task<List<Product>> GetTopProductsAsync()
        {
            var redisKey = "product:10";
            var db = _redis.GetDatabase();
            var cachedData = await db.StringGetAsync(redisKey);

            if (!cachedData.IsNullOrEmpty)
            {
                Console.WriteLine("Redis'ten veri alındı.");
                return JsonSerializer.Deserialize<List<Product>>(cachedData)!;
            }

            var products = await _context.Set<Product>().Take(10).ToListAsync();
            await db.StringSetAsync(redisKey, JsonSerializer.Serialize(products), TimeSpan.FromHours(1));
            Console.WriteLine("PostgreSQL'den veri alındı ve Redis'e yazıldı.");
            return products;
        }


    }
}

