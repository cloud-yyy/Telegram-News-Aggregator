using Microsoft.Extensions.Logging;
using Services.Contracts;
using Shared.Dtos;

namespace Services
{
    public class ApplicationEntryPoint
    {
        private readonly WTelegramClient _telegramClient;
        private readonly ITelegramMessageReader _messageReader;
        private readonly ILogger<ApplicationEntryPoint> _logger;

        public ApplicationEntryPoint(
            WTelegramClient telegramClient,
            MessageLifetimeTracker messageLifetimeTracker,
            ITelegramMessageReader messageReader,
            MessageBrokerConfig config,
            ILogger<ApplicationEntryPoint> logger)
        {
            _telegramClient = telegramClient;
            _messageReader = messageReader;
            _logger = logger;
            
            config.Configure();
        }

        public async Task Entry()
        {
            var user = new MessageReaderUserDto
            (
                Environment.GetEnvironmentVariable("api_id")!,
                Environment.GetEnvironmentVariable("api_hash")!,
                Environment.GetEnvironmentVariable("phone_number")!,
                () =>
                {
                    Console.WriteLine("Code: ");
                    return Console.ReadLine()!;
                }
            );

            var succeed = await _telegramClient.LoginAsync(user);

            if (succeed)
                _logger.LogInformation("TelegramCLient logged in.");
            else
                _logger.LogError("Login failed");

            await _messageReader.StartListeningAsync();
        }
    }
}
