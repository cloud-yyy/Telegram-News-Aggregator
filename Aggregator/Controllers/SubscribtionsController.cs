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
            public enum RequestAction
            {
                Subscribe,
                Unsubscribe
            }

            public RequestAction Action { get; set; }
            public long UserTelegramId { get; set; }
            public long ChannelTelegramId { get; set; }
        }

        private readonly SubscribtionsService _subscribtionsService;

        public SubscribtionsController(SubscribtionsService subscribtionsService)
        {
            _subscribtionsService = subscribtionsService;
        }

        [HttpGet]
        [Route("channels")]
        public async Task<IActionResult> GetAllChannels()
        {
            var channels = await _subscribtionsService.GetChannels();
            return Ok(channels);
        }

        [HttpPost]
        public async Task<IActionResult> Modify(Request request)
        {
            if (request.Action == Request.RequestAction.Subscribe)
                await _subscribtionsService.SubscribeOnChannel(request.UserTelegramId, request.ChannelTelegramId);

            if (request.Action == Request.RequestAction.Unsubscribe)
                await _subscribtionsService.UnsubscribeOfChannel(request.UserTelegramId, request.ChannelTelegramId);

            return Ok();
        }
    }
}
