using Microsoft.Extensions.DependencyInjection;
using Summarizer.Contracts;
using Summarizer.Service;
using Summarizer.Service.Wrappers;

namespace Summarizer.Extensions
{
    public static class ConfigureSummarizerExtension
    {
        public static void ConfigureSummarizer(this IServiceCollection services)
        {
            services.AddSingleton<BufferedBlockService>();
            services.AddSingleton<BufferedMessagesSummarizer>();
            services.AddSingleton<MessageComparerBase, ChatGPTMessagesComparer>();
            services.AddSingleton<IMessagesSummarizer, ChatGPTMessagesSummarizer>();
            services.AddSingleton<IMessageBufferizerService, MessageBufferizerService>();

            services.AddSingleton<IMessageSummarizerService, MessageSummarizerService>();
            services.AddHostedService<MessageSummarizerServiceWrapper>();
        }
    }
}
