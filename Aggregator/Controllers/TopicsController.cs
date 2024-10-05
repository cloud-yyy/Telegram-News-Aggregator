using Microsoft.AspNetCore.Mvc;
using Services.Topics;
using Shared.Dtos;

namespace Aggregator.Controllers
{
    [ApiController]
    [Route("api/topics")]
    public class TopicsController : ControllerBase
    {
        private readonly TopicsService _topicsService;

        public TopicsController(TopicsService topicsService)
        {
            _topicsService = topicsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTopics(long? userId)
        {
            var result = await _topicsService.GetAllTopics(userId);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetTopicById(Guid id)
        {
            var result = await _topicsService.GetTopicById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] TopicDto dto)
        {
            await _topicsService.CreateTopic(dto);
            return Created();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteTopic(Guid id)
        {
            await _topicsService.DeleteTopic(id);
            return NoContent();
        }
    }
}
