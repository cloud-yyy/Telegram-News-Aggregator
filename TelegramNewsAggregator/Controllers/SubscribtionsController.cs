using Microsoft.AspNetCore.Mvc;
using Services.Subscribtions;
using Shared.Dtos;

namespace TelegramNewsAggregator.Controllers
{
    [ApiController]
    [Route("api/subscribtions")]
    public class SubscribtionsController : ControllerBase
    {
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
        public async Task<IActionResult> Subscribe(long userTelegramId, ChannelDto channel)
        {
            // do something with arguments -- combine them into one dto or read if from request string and channel from body

            await _subscribtionsService.SubscribeOnChannel(userTelegramId, channel);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe(long userTelegramId, ChannelDto channel)
        {
            await _subscribtionsService.UnsubscribeOfChannel(userTelegramId, channel);
            return Ok();
        }
    }
}
