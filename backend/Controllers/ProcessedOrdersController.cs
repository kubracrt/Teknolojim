using Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProcessedOrdersController : ControllerBase
    {
        private readonly eCommerceContext _context;
        private readonly KafkaProducerService _kafkaProducerService;

        public ProcessedOrdersController(eCommerceContext context, IConfiguration config)
        {
            _context = context;
            _kafkaProducerService = new KafkaProducerService(config);
        }

        [HttpGet]
        public async Task<IActionResult> GetProcessedOrders()
        {
            var processedOrders = await _context.ProcessedOrders.ToListAsync();
            if (processedOrders.Count > 0)
            {
                return Ok(processedOrders);
            }
            else
            {
                return BadRequest("Processed bulunamadı");
            }

        }       
    }
}