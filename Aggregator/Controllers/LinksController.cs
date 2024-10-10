using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Aggregator.Controllers
{
    [ApiController]
    [Route("/links")]
    public class LinksController : ControllerBase
    {
        private IDbContextFactory<ApplicationContext> _contextFactory;

        public LinksController(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<IActionResult> HandleLink(Guid id)
        {
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
    }
}