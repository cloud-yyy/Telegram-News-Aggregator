using Microsoft.Extensions.DependencyInjection;
using Publisher.Contracts;

namespace Publisher.Extensions
{
    public static class ConfigurePublisherExtension
    {
        public static void ConfigurePublisher(this IServiceCollection services)
        {
            services.AddSingleton<IPublishClient, TelegramBotPublishClient>();
        }
    }
}
