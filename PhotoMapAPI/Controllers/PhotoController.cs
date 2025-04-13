using Microsoft.AspNetCore.Mvc;

namespace PhotoMapAPI.Controllers
{
    [ApiController]
    [Route("api/photos")]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoServices photoServices;

        public PhotoController(IPhotoServices photoServices)
        {
            this.photoServices = photoServices;
        }

        // GET: api/photos/{id}
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
        
        //TODO: UPDATE: api/photos/{id}
    }
}