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
    }
}