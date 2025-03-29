namespace PhotoMapAPI.Models;

public class PointOnMap()
{
    public readonly uint UId;
    public string Name { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<Photo> Photo { get; set; }

    public PointOnMap(string name, string? Description, 
        uint uId, double latitude, double longitude, List<Photo> photo) : this()
    {
        Name = name; 
        UId = uId;
        Latitude = latitude;
        Longitude = longitude;
        Photo = photo;
    }
}