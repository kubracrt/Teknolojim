using Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities;
using Services;
using StackExchange.Redis;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly KafkaProducerService _kafkaProducerService;

        private readonly ILogger<ProductController> _logger;
        
        private readonly RedisProductService _redisProductService;

        public ProductController(ProductService productService, RedisProductService redisProductService, KafkaProducerService kafkaProducerService,ILogger<ProductController> logger)
        {
            _productService = productService;
            _redisProductService = redisProductService;
            _kafkaProducerService = kafkaProducerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetHomePage()
        {
            var redisProducts = await _redisProductService.GetTopProductsAsync();
            if (redisProducts == null || redisProducts.Count == 0)
            {
                return NotFound("Ürün Bulunamadı");
            }

            _logger.LogInformation("Anasafa yüklendi");
            
            return Ok(redisProducts);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            if (products.Count > 0)
            {
                return Ok(products);
            }
            else
            {
                return NotFound("Ürün Bulunamadı");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);

            if (product != null)
            {
                

                await _kafkaProducerService.SendProductViewAsync(product.Id.ToString(), product.Name, product.ImageUrl);
                return Ok(product);
            }
            else
            {
                return NotFound("Ürün Bulunamadı");
            }
        }

        // [HttpGet]
        // public async Task<IActionResult> GetTopProducts()
        // {
        //     var topProducts = await _productService.GetTop10Products();
        //     if (topProducts != null && topProducts.Count > 0)
        //     {
        //         return Ok(topProducts);
        //     }
        //     else
        //     {
        //         return NotFound("Ürün Bulunamadı");
        //     }
        // }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProductAdmin(int userId)
        {
            var adminProducts = await _productService.GetProductAdminAsync(userId);
            if (adminProducts != null && adminProducts.Count > 0)
            {
                return Ok(adminProducts);
            }
            else
            {
                return NotFound("Ürün Bulunamadı");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProducts([FromBody] Product product)
        {
            var addedProduct = await _productService.AddProductAsync(product);
            return Ok(new { message = "Ürün Eklendi", addedProduct });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.DeleteProductAsync(id);
            if (product == null)
            {
                return NotFound("Ürün Bulunamadı");
            }

            return Ok(new { message = "Ürün silindi", product });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] Product updateProduct)
        {
            var product = await _productService.UpdateProductAsync(id, updateProduct);
            if (product == null)
            {
                return NotFound("Ürün Bulunamadı");
            }

            return Ok(new { message = "Ürün güncellendi", product });
        }
    }
}
