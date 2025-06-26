using PhotoMapAPI.Models;

public interface IAvatarService
{
    Task<string> UploadAvatarAsync(IFormFile file, string userId);
    Task<bool> DeleteAvatarAsync(string userId);
    Task<string?> GetAvatarPathAsync(string userId);
}