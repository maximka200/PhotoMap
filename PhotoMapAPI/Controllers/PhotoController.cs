using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PhotoMapAPI.Controllers
{
    [ApiController]
    [Route("api/photos")]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoServices photoServices;
        private readonly UserManager<User> userManager;

        public PhotoController(IPhotoServices photoServices, UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.photoServices = photoServices;
        }

        // GET: api/photos/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhotoById(uint id)
        {
            var photo = await photoServices.GetPhotoById(id);
            if (photo == null)
            {
                return NotFound($"Photo with ID {id} not found.");
            }
            return Ok(photo);
        }
        
        // DELETE: api/photos/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePhoto(uint id)
        {
            try
            {
                await photoServices.DeletePhoto(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Photo with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
        
        // DELETE: api/photos/like/{id}
        [HttpPost("like/{photoId}")]
        [Authorize]
        public async Task<IActionResult> LikePhoto(uint photoId)
        {
            try
            {
                var user = User;
                var userId = userManager.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }
                await photoServices.LikePhoto(photoId, userId);
                return Ok($"Photo with ID {photoId} liked successfully by {userId}.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Photo with ID {photoId} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
        
        // DELETE: api/photos/dislike/{id}
        [HttpDelete("dislike/{photoId}")]
        [Authorize]
        public async Task<IActionResult> DislikePhoto(uint photoId)
        {
            try
            {
                var user = User;
                var userId = userManager.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }
                await photoServices.DislikePhoto(photoId, userId);
                return Ok($"Photo with ID {photoId} liked successfully by {userId}.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Photo with ID {photoId} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
        //TODO: UPDATE: api/photos/{id}
    }
}