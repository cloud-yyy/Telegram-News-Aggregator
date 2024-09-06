using NLog;
using NLog.Web;

namespace Aggregator.Extensions
{
    public static class BuilderExtensions
    {
        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseNLog();
        }
    }
}
