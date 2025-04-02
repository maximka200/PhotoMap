using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace PhotoMapAPI.Models;

public class Photo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint UId { get; private set; }
    public string Path => this.GetPhotoPath();

    public Photo(uint uId)
    {
        UId = uId;
    }
    
    private string GetPhotoPath()
        => $"{UId.ToString()}{FileExtensions.Jpg}";
}