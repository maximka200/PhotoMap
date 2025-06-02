using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using PhotoMapAPI.Services;

namespace PhotoMapAPI.Controllers
{
    [ApiController]
    [Route("api/user/")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;

        public UserController(IUserService userService, UserManager<User> userManager)
        {
            this.userService = userService;
            this.userManager = userManager;
        }
        
        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        
        // GET: api/user/all
        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userService.GetAllUsersAsync();
            return Ok(users);
        }
        
        // GET: api/user/liked
        [HttpGet("liked")]
        [Authorize]
        public async Task<IActionResult> GetLikedPhotoIds()
        {
            var userId = userManager.GetUserId(User);
            var photos = await userService.GetLikedPhotoIdsAsync(userId);
            return Ok(photos);
        }
    }
}