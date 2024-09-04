using NLog;
using NLog.Web;

namespace Aggregator.Extensions
{
    public static class BuilderExtensions
    {
        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("NLog logger added");

            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
        }
    }
}
