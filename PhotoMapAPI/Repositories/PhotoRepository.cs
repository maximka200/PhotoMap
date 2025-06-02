using Microsoft.EntityFrameworkCore;
using PhotoMapAPI.Data;
using PhotoMapAPI.Services;

namespace PhotoMapAPI.Repositories;

public class PhotoRepository : Repository<Photo>, IPhotoRepository
{
    public PhotoRepository(ApplicationDbContext context, ILogger<PhotoRepository> logger) : base(context, logger)
    {
    }

    public async Task<List<Photo>> GetAllInPointAsync(uint pointId)
    {
        var photos = await dbSet.Where(p => p.PointId == pointId).ToListAsync();
        
        foreach (var photo in photos)
        {
            photo.Point = null;
        }

        return photos;
    }

    public async Task LikePhotoFromUserAsync(uint photoId, string userId)
    {
        var user = await context.Users
            .Include(u => u.LikedPhotos)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("User not found");

        var photo = await context.Photos.FirstOrDefaultAsync(p => p.UId == photoId);

        if (photo == null)
            throw new Exception("Photo not found");
        
        if (!user.LikedPhotos.Any(p => p.UId == photoId))
        {
            user.LikedPhotos.Add(photo);
            await context.SaveChangesAsync();
        }
    }
    
    public async Task DislikePhotoFromUserAsync(uint photoId, string userId)
    {
        var user = await context.Users
            .Include(u => u.LikedPhotos)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("User not found");

        var likedPhoto = user.LikedPhotos.FirstOrDefault(p => p.UId == photoId);

        if (likedPhoto != null)
        {
            user.LikedPhotos.Remove(likedPhoto);
            await context.SaveChangesAsync();
        }
    }
}