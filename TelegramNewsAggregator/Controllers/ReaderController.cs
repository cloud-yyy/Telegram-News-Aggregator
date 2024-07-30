using Microsoft.AspNetCore.Mvc;

namespace TelegramNewsAggregator
{
    [ApiController]
    [Route("/")]
    public class ReaderController : ControllerBase
    {
        private readonly ITelegramClient _telegramClient;

        public ReaderController(ITelegramClient telegramClient)
        {
            _telegramClient = telegramClient;
        }

        [HttpGet]
        public async Task<IActionResult> StartListening()
        {
            var user = new UserDto
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

            if (succeed)
            {
                await _telegramClient.HandleNewMessagesAsync();
                return Ok("Listening was started...");
            }
            else
            {
                return BadRequest("Login wasn't succeed.");
            }            
        }
    }
}
