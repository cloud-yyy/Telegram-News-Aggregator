using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Aggregator.Controllers
{
    [ApiController]
    [Route("metrics")]
    public class MetricsController : ControllerBase
    {
        public new class Request
        {
            public long UserTelegramId { get; set; }
            public string Action { get; set; } = string.Empty;
        }

        private readonly IDbContextFactory<ApplicationContext> _contextFactory;

        public MetricsController(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [HttpPost]
        [Route("api/signal")]
        public async Task<IActionResult> HandleMetrics([FromBody] Request request)
        {
            var signal = new MetricsSignal()
            {
                Id = Guid.NewGuid(),
                Action = request.Action,
                UserTelegramId = request.UserTelegramId,
                ClickedAt = DateTime.UtcNow
            };

            using var context = _contextFactory.CreateDbContext();
            context.MetricsSignals.Add(signal);
            await context.SaveChangesAsync();

            return Ok();
        } 
    }
}
