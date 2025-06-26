namespace PhotoMapAPI.Services;

public interface IPhotoRepository : IRepository<Photo>
{
    Task<List<Photo>> GetAllInPointAsync(uint pointId);
    Task<List<string>> GetIdUserThatLike(uint id);
    Task LikePhotoFromUserAsync(uint id, string userId);
    Task DislikePhotoFromUserAsync(uint id, string userId);
}