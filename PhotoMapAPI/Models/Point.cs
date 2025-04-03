using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoMapAPI.Models
{
    public class Point
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint UId { get; private set; }
        
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<Photo> Photo { get; set; }

        public Point()
        {
            // empty builder
        }
        
        public Point(string name, string? description, double latitude, double longitude, List<Photo> photo)
        {
            Name = name; 
            Latitude = latitude;
            Longitude = longitude;
            Photo = photo;
        }
    }
}