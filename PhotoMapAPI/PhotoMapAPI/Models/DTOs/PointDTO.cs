using System.ComponentModel.DataAnnotations;

namespace PhotoMapAPI.DTOs
{
    public class PointCreateDto
    {
        [Required]
        public string Name { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
    }
}