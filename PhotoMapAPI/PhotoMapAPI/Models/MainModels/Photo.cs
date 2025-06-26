using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using PhotoMapAPI.Models;

public class Photo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint UId { get; private set; }

    public string Url { get; set; }

    [ForeignKey("Point")]
    public uint PointId { get; set; }
    [JsonIgnore]
    public Point Point { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public List<User> LikedByUsers { get; set; } = new();
    
    [NotMapped]
    public List<string> LikedIds { get; set; }
    
    public Photo(string url, uint pointId)
    {
        Url     = url;
        PointId = pointId;
    }
}
