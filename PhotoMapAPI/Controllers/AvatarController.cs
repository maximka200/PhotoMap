using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhotoMapAPI.Models;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AvatarController : ControllerBase
{
    private readonly IAvatarService _avatarService;
    private readonly UserManager<User> _userManager;

    public AvatarController(
        IAvatarService avatarService,
        UserManager<User> userManager)
    {
        _avatarService = avatarService;
        _userManager = userManager;
    }

    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var avatarPath = await _avatarService.UploadAvatarAsync(file, userId);
            return Ok(new { Path = avatarPath });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteAvatar()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _avatarService.DeleteAvatarAsync(userId);
        return result ? Ok() : BadRequest("Avatar not found");
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetAvatar(string userId)
    {
        var path = await _avatarService.GetAvatarPathAsync(userId);
        if (path == null) return NotFound();

        return Ok(new { Path = path });
    }
}