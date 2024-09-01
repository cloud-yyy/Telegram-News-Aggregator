using Microsoft.AspNetCore.Mvc;
using Services.Channels;
using Shared.Dtos;

namespace TelegramNewsAggregator.Controllers
{
    [ApiController]
    [Route("api/channels")]
    public class ChannelsController : ControllerBase
    {
        private readonly ChannelService _service;

        public ChannelsController(ChannelService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllChannels()
        {
            var channels = await _service.GetAllChannelsAsync();
            return Ok(channels);
        }

        [HttpGet("{id:long}", Name = "GetChannel")]
        public async Task<IActionResult> GetChannel(Guid id)
        {
            var channel = await _service.GetChannelAsync(id);
            return Ok(channel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel([FromBody] ChannelForCreationDto channelForCreationDto)
        {
            var result = await _service.CreateChannelAsync(channelForCreationDto);
            return CreatedAtRoute("GetChannel", new { result.Id }, result);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteChannel(Guid id)
        {
            await _service.DeleteChannelAsync(id);
            return NoContent();
        }
    }
}
