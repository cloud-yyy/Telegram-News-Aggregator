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
        private readonly IDbContextFactory<ApplicationContext> _contextFactory;

        public LinksMetricsController(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> Handle(Guid id)
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

            return RedirectPermanent(link.InnerLink);
        }
    }
}
