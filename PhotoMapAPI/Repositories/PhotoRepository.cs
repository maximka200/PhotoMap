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
}