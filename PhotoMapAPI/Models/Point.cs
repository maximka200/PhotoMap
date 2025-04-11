using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PhotoMapAPI.Models
{
    public class Point(string name, string? description, double latitude, double longitude)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint UId { get; private set; }

        public string Name { get; set; } = name;
        public string? Description { get; set; } = description;
        public double Latitude { get; set; } = latitude;
        public double Longitude { get; set; } = longitude;

        [JsonIgnore]
        public List<Photo>? Photos { get; set; } = new();
        public void AddPhoto(Photo photo)
        {
            if (Photos == null)
            {
                Photos = new List<Photo>();
            }
            Photos.Add(photo);
        }
    }
}