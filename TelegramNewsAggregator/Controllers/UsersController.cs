using Microsoft.AspNetCore.Mvc;
using Services;
using Shared.Dtos;

namespace TelegramNewsAggregator.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            await _userService.CreateUserAsync(userDto);
            return Ok();
        }
    }
}
