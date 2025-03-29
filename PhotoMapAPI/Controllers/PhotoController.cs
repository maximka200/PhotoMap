using Microsoft.AspNetCore.Mvc;

namespace PhotoMapAPI.Controllers;

[ApiController]
[Route("api/photos/[controller]")]
public class PhotoController : ControllerBase
{
    private readonly IPhotoServices photoServices;

    public PhotoController(IPhotoServices photoServices)
    {
        this.photoServices = photoServices;
    }
    
    [HttpGet("{id}")]
    public IActionResult GetPhotoById(int id)
    {
        var photo = photoServices.GetPhotoById(id);
        if (photo == null)
        {
            return NotFound($"Photo with ID {id} not found.");
        }
        return Ok(photo);
    }
}