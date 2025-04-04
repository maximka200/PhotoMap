namespace PhotoMapAPI.Controllers;

public interface IPhotoUploadServices
{
    Task<string> UploadPhotoAsync(IFormFile file, uint pointId);
}