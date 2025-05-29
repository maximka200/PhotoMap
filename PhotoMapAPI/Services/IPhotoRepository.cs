namespace PhotoMapAPI.Services;

public interface IPhotoRepository : IRepository<Photo>
{
    Task<List<Photo>> GetAllInPointAsync(uint pointId);
    Task LikePhotoFromUserAsync(uint id, string userId);
}