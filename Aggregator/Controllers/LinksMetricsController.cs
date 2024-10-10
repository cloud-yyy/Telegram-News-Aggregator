using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Aggregator.Controllers
{
    [ApiController]
    [Route("metrics")]
    public class LinksMetricsController : ControllerBase
    {
        public new class Request
        {
            public long UserTelegramId { get; set; }
            public string Action { get; set; } = string.Empty;
        }

        private readonly IDbContextFactory<ApplicationContext> _contextFactory;

        public LinksMetricsController(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [HttpGet]
        [Route("/links/{id:guid}")]
        public async Task<IActionResult> HandleLink(Guid id)
        {
            // TODO: remove it
            using var context = _contextFactory.CreateDbContext();

            var link = await context.WrappedLinks.FindAsync(id);

            if (link == null)
                return NotFound();

            var signal = new MetricsSignal()
            {
                Id = Guid.NewGuid(),
                Action = MetricsSignal.Type.ClickedOriginLink.ToString(),
                UserTelegramId = 0,
                ClickedAt = DateTime.UtcNow
            };

            context.MetricsSignals.Add(signal);
            await context.SaveChangesAsync();

            return Redirect(link.InnerLink);
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
