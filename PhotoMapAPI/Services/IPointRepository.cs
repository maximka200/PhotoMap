using PhotoMapAPI.Models;
using PhotoMapAPI.Services;

namespace PhotoMapAPI.Repositories;

public interface IPointRepository : IRepository<Point>
{
    Task<List<Point>> GetAllAsync();
}