using PhotoMapAPI.Controllers;
using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;

namespace PhotoMapAPI.Services;

public class PointServices : IPointServices
{
    private readonly IRepository<Point> repository;
    
    public PointServices(IRepository<Point> repository)
    {
        this.repository = repository;
    }
    public async Task<List<Point>?> GetAllPointsInEkaterinburg()
    {
        return await repository.GetAllAsync();
    }
    public async Task<Point?> GetPointById(uint id)
    {
        return await repository.GetByIdAsync(id);
    }
    public async Task AddPoint(Point point)
    {
        await repository.AddAsync(point);
    }
}