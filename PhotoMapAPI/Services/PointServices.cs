using PhotoMapAPI.Controllers;
using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;

namespace PhotoMapAPI.Services;

public class PointServices : IPointServices
{
    private readonly Repository<Point> repository;
    public async Task<List<Point>?> GetAllPointsInEkaterinburg()
    {
        return await repository.GetAllAsync();
    }

    public async Task<Point?> GetPointById(uint id)
    {
        return await repository.GetByIdAsync(id);
    }
}