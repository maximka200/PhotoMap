namespace PhotoMapAPI.Controllers;

public interface IPhotoServices
{
    Models.Photo GetPhotoById(int id);
}