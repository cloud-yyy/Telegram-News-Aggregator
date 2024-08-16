using Services;
using Services.Contracts;

namespace TelegramNewsAggregator
{
    public static class ServicesExtensions
    {
        public static void ConfigureSummarizing(this IServiceCollection services)
        {
            services.AddScoped<MessageBufferizerService>();
            services.AddScoped<MessageComparerBase, ChatGPTMessagesComparer>();
            services.AddScoped<IMessagesSummarizer, ChatGPTMessagesSummarizer>();
            services.AddScoped<BufferedMessagesSummarizer>();
            services.AddScoped<BufferedBlockService>();
            services.AddScoped<MessageStatusDbWriter>();
            services.AddScoped<MessageLifetimeTracker>();
        }

        public static void ConfigureMessagesReading(this IServiceCollection services)
        {
            services.AddScoped<ITelegramMessageReader, WTelegramMessageReader>();
            services.AddScoped<ITagsExtractService, ChatGPTTagsExtractService>();
            services.AddScoped<MessageWithTagsSaver>();
            services.AddScoped<MessageDbWriter>();
            services.AddScoped<TagsDbWriter>();
        }
    }
}
