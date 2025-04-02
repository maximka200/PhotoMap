using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;

namespace PhotoMapAPI.Controllers;

public interface IPointServices
{
    Task<List<Point>?> GetAllPointsInEkaterinburg();
    Task<Point?> GetPointById(uint id);
}