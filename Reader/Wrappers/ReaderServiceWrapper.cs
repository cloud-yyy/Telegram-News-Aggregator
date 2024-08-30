using Microsoft.Extensions.Hosting;
using Reader.Service;

namespace Reader.Wrappers
{
    internal class ReaderServiceWrapper : BackgroundService
    {
        private readonly IReaderService _readerService;

        public ReaderServiceWrapper(IReaderService readerService)
        {
            _readerService = readerService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _readerService.StartReadingAsync();
        }
    }
}