using Microsoft.AspNetCore.Mvc;
using PhotoMapAPI.Models;

namespace PhotoMapAPI.Controllers
{
    [ApiController]
    [Route("api/points")]
    public class PointController : ControllerBase
    {
        private readonly IPointServices pointServices;

        public PointController(IPointServices pointServices)
        {
            this.pointServices = pointServices;
        }

        // GET: api/points/ekaterinburg
        [HttpGet("ekaterinburg")]
        public async Task<IActionResult> GetAllPointsInEkaterinburg()
        {
            var points = await pointServices.GetAllPointsInEkaterinburg();
            if (points == null || points.Count == 0)
            {
                return NotFound("No points found in Ekaterinburg.");
            }
            return Ok(points);
        }

        // GET: api/points/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPointById(uint id)
        {
            var point = await pointServices.GetPointById(id);
            if (point == null)
            {
                return NotFound($"Point with ID {id} not found.");
            }
            return Ok(point);
        }
    }
}