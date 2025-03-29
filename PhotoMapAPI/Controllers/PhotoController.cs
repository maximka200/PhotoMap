using Microsoft.AspNetCore.Mvc;
using PhotoMapAPI.Models;

namespace PhotoMapAPI.Controllers
{
    [ApiController]
    [Route("api/points")]
    public class PointController : ControllerBase
    {
        private static readonly List<PointOnMap> Points = new()
        {
            new PointOnMap("Point A", "Description A", 1, 55.751244, 37.618423, new List<Photo> { new Photo(1) }),
            new PointOnMap("Point B", "Description B", 2, 40.712776, -74.005974, new List<Photo> { new Photo(2) })
        };

        [HttpGet]
        public IActionResult GetAllPoints()
        {
            return Ok(Points);
        }

        [HttpGet("{id}")]
        public IActionResult GetPoint(uint id)
        {
            var point = Points.FirstOrDefault(p => p.UId == id);
            if (point == null)
                return NotFound(new { Message = "Point not found" });
            
            return Ok(point);
        }
    }
}