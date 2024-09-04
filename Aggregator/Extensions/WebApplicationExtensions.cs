using Microsoft.EntityFrameworkCore;
using Repository;

namespace Aggregator.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void EnsureDatabaseCreated(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            using (var context = scope.ServiceProvider
                .GetRequiredService<IDbContextFactory<ApplicationContext>>().CreateDbContext())
            {
                context.Database.EnsureCreated();
            }
        }

        public static void ConfigureBroker(this WebApplication app)
        {
            var config = app.Services.GetRequiredService<BrokerConfig>();
            config.Configure();
        }
    }
}