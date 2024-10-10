using Microsoft.AspNetCore.Mvc;
using Services.Users;
using Shared.Dtos;

namespace Aggregator.Controllers
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

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var result = _userService.GetAllUsers();
            return Ok(result);
        }

        [HttpGet]
        [Route("{telegramId:long}")]
        public async Task<IActionResult> GetUserById(long telegramId)
        {
            var result = await _userService.GetUserById(telegramId);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            await _userService.CreateUserAsync(userDto);
            return Ok();
        }
    }
}
