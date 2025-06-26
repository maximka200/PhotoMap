using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoMapAPI.Models;
using PhotoMapAPI.DTOs;

namespace PhotoMapAPI.Controllers
{
    [ApiController]
    [Route("api/points")]
    public class PointController : ControllerBase
    {
        private readonly IPointServices pointServices;
        private readonly ILogger<PointController> logger;

        public PointController(IPointServices pointServices, ILogger<PointController> logger)
        {
            this.pointServices = pointServices;
            this.logger = logger;
        }
        
        // POST: api/photos/ekaterinburg
        [HttpGet("ekaterinburg")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPointsInEkaterinburg()
        {
            try
            {
                var points = await pointServices.GetAllPointsInEkaterinburg();
                return points == null || !points.Any() 
                    ? NotFound("No points found in Ekaterinburg.") 
                    : Ok(points);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting points in Ekaterinburg");
                return StatusCode(500, "Internal server error");
            }
        }
        
        // GET: api/photos/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPointById(uint id)
        {
            try
            {
                var point = await pointServices.GetPointById(id);
                return point == null 
                    ? NotFound($"Point with ID {id} not found.") 
                    : Ok(point);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error getting point with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }
        
        // POST: api/photos
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPoint([FromBody] PointCreateDto pointDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var point = new Point(
                    name: pointDto.Name,
                    description: pointDto.Description,
                    latitude: pointDto.Latitude,
                    longitude: pointDto.Longitude
                );

                await pointServices.AddPoint(point);
                return CreatedAtAction(nameof(GetPointById), new { id = point.UId }, point);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding new point");
                return StatusCode(500, "Internal server error");
            }
        }
        
        // DELETE: api/photos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePoint(uint id)
        {
            try
            {
                await pointServices.DeletePoint(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error deleting point with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}