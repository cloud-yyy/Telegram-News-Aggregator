using Microsoft.Extensions.DependencyInjection;
using Reader.Contracts;
using Reader.Service;
using Reader.Wrappers;
using Services;

namespace Reader.Extensions
{
    public static class ConfigureReaderExtension
    {
        public static void ConfigureReader(this IServiceCollection services)
        {
            services.AddSingleton<IMessageSaver, MessageWithTagsSaver>();
            services.AddSingleton<ITagsExtractService, ChatGPTTagsExtractService>();

            services.AddSingleton<IReaderService, WTelegramReaderService>();
            services.AddHostedService<ReaderServiceWrapper>();
        }
    }
}
