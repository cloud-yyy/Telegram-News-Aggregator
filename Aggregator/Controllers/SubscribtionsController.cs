using Microsoft.AspNetCore.Mvc;
using Services.Subscribtions;

namespace Aggregator.Controllers
{
    [ApiController]
    [Route("api/subscribtions")]
    public class SubscribtionsController : ControllerBase
    {
        public new class Request
        {
            public long UserTelegramId { get; set; }
            public List<Guid> TopicIds { get; set; } = [];
        }

        private readonly SubscribtionsService _subscribtionsService;

        public SubscribtionsController(SubscribtionsService subscribtionsService)
        {
            _subscribtionsService = subscribtionsService;
        }

        [HttpPost]
        [Route("subscribe")]
        public async Task<IActionResult> SubscribeOnTopic(Request request)
        {
            foreach (var id in request.TopicIds)
                await _subscribtionsService.SubscribeOnTopic(request.UserTelegramId, id);

            return Ok();
        }

        [HttpPost]
        [Route("unsubscribe")]
        public async Task<IActionResult> UnsubscribeOnTopic(Request request)
        {
            foreach (var id in request.TopicIds)
                await _subscribtionsService.UnsubscribeOnTopic(request.UserTelegramId, id);

            return NoContent();
        }
    }
}
