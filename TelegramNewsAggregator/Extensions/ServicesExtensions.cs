using Services;
using Services.Contracts;

namespace TelegramNewsAggregator
{
    public static class ServicesExtensions
    {
        public static void ConfigureSummarizing(this IServiceCollection services)
        {
            services.AddScoped<SummarizingEntryPoint>();
            services.AddScoped<IMessageComparer, ChatGPTMessagesComparer>();
            services.AddScoped<IMessagesSummarizer, ChatGPTMessagesSummarizer>();
            services.AddScoped<MessagesTagsDbReader>();
            services.AddScoped<MessageDbReader>();
            services.AddScoped<MessageStatusDbWriter>();
            services.AddScoped<SummaryDbWriter>();
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