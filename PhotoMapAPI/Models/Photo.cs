using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PhotoMapAPI.Models;

public class Photo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint UId { get; private set; }

    public string Url { get; set; }
    
    [ForeignKey("Point")]
    public uint PointId { get; set; }
    public Point Point { get; set; }
    
    public Photo(string url, uint pointId)
    {
        Url = url;
        PointId = pointId;
    }
}