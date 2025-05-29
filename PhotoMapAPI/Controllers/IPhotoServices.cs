using PhotoMapAPI.Models;

namespace PhotoMapAPI.Controllers;

public interface IPhotoServices
{
    Task<Photo?> GetPhotoById(uint id);
    Task DeletePhoto(uint id);
    Task LikePhoto(uint id, string user);
}