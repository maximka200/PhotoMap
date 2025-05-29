using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PhotoMapAPI.Services;

namespace PhotoMapAPI.Controllers
{
    [ApiController]
    [Route("api/user/")]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userService.GetAllUsersAsync();
            return Ok(users);
        }

        // [HttpGet("liked")]
        // public async Task<IActionResult> GetLikedPhotoIds(uint photoId)
        // {
        //     var users = await userService.GetLikedPhotoIdsAsync(photoId);
        //     if (users == null || users.Count == 0)
        //         return NotFound($"No users found who liked photo with ID {photoId}.");
        //
        //     return Ok(users);
        // }
    }
}