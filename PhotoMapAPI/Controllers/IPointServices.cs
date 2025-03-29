using PhotoMapAPI.Models;

namespace PhotoMapAPI.Controllers;

public interface IPointServices
{
    List<Point> GetAllPointsInEkaterinburg();
    Point GetPointById(int id);
}