using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;

namespace PhotoMapAPI.Controllers;

public interface IPointServices
{
    Task<List<Point>?> GetAllPointsInEkaterinburg();
    Task<Point?> GetPointById(uint id);
    Task AddPoint(Point point);
    Task DeletePoint(uint id);
}