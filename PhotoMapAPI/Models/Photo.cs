using System.Xml.Linq;

namespace PhotoMapAPI.Models;

public class Photo
{
    public readonly uint UId;
    public string Path => this.GetPhotoPath();

    public Photo(uint uId)
    {
        UId = uId;
    }
    
    private string GetPhotoPath()
        => $"{UId.ToString()}{FileExtensions.Jpg}";
}