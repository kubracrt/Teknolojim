using Microsoft.AspNetCore.Mvc;
using Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class ViewEventController : ControllerBase
    {
        private readonly ViewEventService _viewEventService;

        public ViewEventController(ViewEventService viewEventService)
        {
            _viewEventService = viewEventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetViewEvents()
        {
            var viewEvents = await _viewEventService.GetLast30MinViewsAsync();
            if (viewEvents.Count > 0)
            {
                return Ok(viewEvents);
            }
            else
            {
                return NotFound("Görüntüleme Olayı Bulunamadı");
            }
         
        }
    }
}