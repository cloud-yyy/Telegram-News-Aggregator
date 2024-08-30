using Microsoft.Extensions.Hosting;
using Summarizer.Contracts;

namespace Summarizer.Service.Wrappers;

internal class MessageSummarizerServiceWrapper : BackgroundService
{
    private readonly IMessageSummarizerService _summarizerService;

    public MessageSummarizerServiceWrapper(IMessageSummarizerService summarizerService)
    {
        _summarizerService = summarizerService;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _summarizerService.StartAsync();
    }
}
