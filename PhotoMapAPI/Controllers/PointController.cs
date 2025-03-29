using Microsoft.AspNetCore.Mvc;
using PhotoMapAPI.Models;

namespace PhotoMapAPI.Controllers
{
    [ApiController]
    [Route("api/points/[controller]")]
    public class PointController : ControllerBase
    {
        private IPointServices pointServices;

        public PointController(IPointServices pointServices)
        {
            this.pointServices = pointServices;
        }
        
        public IActionResult GetAllPointsInEkaterinburg()
        {
            var points = pointServices.GetAllPointsInEkaterinburg();
            if (points == null || points.Count == 0)
            {
                return NotFound("No points found in Ekaterinburg.");
            }
            return Ok(points);
        }

        public IActionResult GetPointById(int id)
        {
            var point = pointServices.GetPointById(id);
            if (point == null)
            {
                return NotFound($"Point with ID {id} not found.");
            }
            return Ok(point);
        }
    }
}