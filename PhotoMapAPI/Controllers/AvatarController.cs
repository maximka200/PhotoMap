using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhotoMapAPI.Models;
using System.Threading.Tasks;

[ApiController]
[Route("api/avatar/")]
public class AvatarController : ControllerBase
{
    private readonly IAvatarService avatarService;
    private readonly UserManager<User> userManager;

    public AvatarController(
        IAvatarService avatarService,
        UserManager<User> userManager)
    {
        this.avatarService = avatarService;
        this.userManager = userManager;
    }
    
    [HttpPost("upload")]
    // [Authorize]
    public async Task<IActionResult> UploadAvatar(IFormFile file, [FromQuery] string userId)
    {
        try
        {
            var currentUserId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
                userId = "0";

            if (currentUserId != userId)
                return StatusCode(403, "You can upload avatar only for yourself.");

            var fileExt = Path.GetExtension(file.FileName);
            if (!(fileExt == ".jpeg" || fileExt == ".jpg"))
                throw new ArgumentException("Only .jpg files are allowed.");

            var avatarPath = await avatarService.UploadAvatarAsync(file, userId);
            
            return Ok(new { Path = avatarPath });
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Invalid file: {ex.Message}");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound("User not found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteAvatar()
    {
        var userId = userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await avatarService.DeleteAvatarAsync(userId);
        return result ? Ok() : BadRequest("Avatar not found");
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetAvatar(string userId)
    {
        var path = await avatarService.GetAvatarPathAsync(userId);
        if (path == null) return NotFound();

        return Ok(new { Path = path });
    }
}