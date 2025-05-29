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
            throw new Exception("Пользователь не найден");

        var photo = await context.Photos.FirstOrDefaultAsync(p => p.UId == photoId);

        if (photo == null)
            throw new Exception("Фото не найдено");
        
        if (!user.LikedPhotos.Any(p => p.UId == photoId))
        {
            user.LikedPhotos.Add(photo);
            await context.SaveChangesAsync();
        }
    }
}