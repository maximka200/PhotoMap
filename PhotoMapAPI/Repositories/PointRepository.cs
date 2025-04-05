using Microsoft.EntityFrameworkCore;
using PhotoMapAPI.Data;
using PhotoMapAPI.Models;

namespace PhotoMapAPI.Repositories;

public class PointRepository : Repository<Point>, IPointRepository
{
    public PointRepository(ApplicationDbContext context, ILogger<PointRepository> logger) : base(context, logger)
    {
    }

    public async Task<List<Point>> GetAllAsync()
    {
        return await context.Points.ToListAsync();
    }
}