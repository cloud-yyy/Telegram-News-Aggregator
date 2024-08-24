using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Contracts;
using Shared.Dtos;

namespace TelegramNewsAggregator
{
    [ApiController]
    [Route("/")]
    public class RootController : ControllerBase
    {
        private readonly WTelegramClient _telegramClient;
        private readonly MessageLifetimeTracker _messageLifetimeTracker;
        private readonly ITelegramMessageReader _messageReader;

        public RootController(
            WTelegramClient telegramClient,
            MessageLifetimeTracker messageLifetimeTracker, 
            ITelegramMessageReader messageReader, 
            MessageBrokerConfig config)
        {
            _telegramClient = telegramClient;
            _messageLifetimeTracker = messageLifetimeTracker;
            _messageReader = messageReader;
            config.Configure();
        }

        [HttpGet]
        public async Task<IActionResult> StartListening()
        {
            var user = new MessageReaderUserDto
            (
                Environment.GetEnvironmentVariable("api_id"),
                Environment.GetEnvironmentVariable("api_hash"),
                Environment.GetEnvironmentVariable("phone_number"),
                () =>
                {
                    Console.WriteLine("Code: ");
                    return Console.ReadLine();
                }
            );

            var succeed = await _telegramClient.LoginAsync(user);
            _messageLifetimeTracker.ExecuteAsync();

            if (succeed)
            {
                await _messageReader.StartListeningAsync();
                return Ok("Listening was started...");
            }
            else
            {
                return BadRequest("Login wasn't succeed.");
            }            
        }
    }
}
