using PhotoMapAPI.Controllers;
using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;

namespace PhotoMapAPI.Services;

public class PointServices : IPointServices
{
    private readonly IPointRepository repositoryPoint;
    private readonly IPhotoRepository repositoryPhoto;
    private readonly ILogger<PointServices> logger;
    
    public PointServices(IPointRepository repositoryPoint, IPhotoRepository repositoryPhoto, ILogger<PointServices> logger)
    {
        this.logger = logger;
        this.repositoryPoint = repositoryPoint;
        this.repositoryPhoto = repositoryPhoto;
    }
    public async Task<List<Point>?> GetAllPointsInEkaterinburg()
    {
        logger.Log(LogLevel.Information,$"{nameof(GetAllPointsInEkaterinburg)} called");
        var allPoints = await repositoryPoint.GetAllAsync();
        foreach (var point in allPoints)
        {
            var photos = await repositoryPhoto.GetAllInPointAsync(point.UId);
            point.Photos = photos;
        }

        return allPoints;
    }
    
    public async Task<Point?> GetPointById(uint id)
    {
        logger.Log(LogLevel.Information,$"{nameof(GetPointById)} called with id: {id}");
        var point = await repositoryPoint.GetByIdAsync(id);
        if (point == null)
        {
            logger.Log(LogLevel.Warning,$"{nameof(GetPointById)} point with id: {id} not found");
            return null;
        }
        point.Photos = await repositoryPhoto.GetAllInPointAsync(point.UId);
        return point;
    }
    public async Task AddPoint(Point point)
    {
        logger.Log(LogLevel.Information,$"{nameof(AddPoint)} called with point: {point.UId}");
        await repositoryPoint.AddAsync(point);
    }
}