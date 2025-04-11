using Microsoft.AspNetCore.Mvc;
using PhotoMapAPI.Services;

namespace PhotoMapAPI.Controllers
{
    [ApiController]
    [Route("api/photos")]
    public class PhotoUploadController : ControllerBase
    {
        private readonly IPhotoUploadServices photoServices;

        public PhotoUploadController(IPhotoUploadServices photoServices)
        {
            this.photoServices = photoServices;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] uint pointId)
        {
            try
            {
                if (file == null || !Path.GetExtension(file.FileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("only .jpg");
                }
                var photoUrl = await photoServices.UploadPhotoAsync(file, pointId);
                return Ok(new { Url = photoUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid format: {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound("Point not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
    }
}