namespace PhotoMapAPI.Models;

public class Point
{
    public readonly uint UId;
    public string Name { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<Photo> Photo { get; set; }

    public Point(string name, string? description, uint uId, double latitude, double longitude, List<Photo> photo)
    {
        Name = name; 
        UId = uId;
        Latitude = latitude;
        Longitude = longitude;
        Photo = photo;
    }
}