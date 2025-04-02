using PhotoMapAPI.Models;

namespace PhotoMapAPI.Controllers;

public interface IPhotoServices
{
    Task<Photo?> GetPhotoById(uint id);
}