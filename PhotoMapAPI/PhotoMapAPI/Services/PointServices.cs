using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using PhotoMapAPI.Controllers;

namespace PhotoMapAPI.Services;

public class PointServices : IPointServices
{
    private readonly IPointRepository repositoryPoint;
    private readonly IPhotoRepository repositoryPhoto;
    private readonly ILogger<PointServices> logger;
    private readonly IWebHostEnvironment env;
    
    public PointServices(
        IPointRepository repositoryPoint, 
        IPhotoRepository repositoryPhoto,
        ILogger<PointServices> logger, 
        IWebHostEnvironment env)
    {
        this.logger = logger;
        this.repositoryPoint = repositoryPoint;
        this.repositoryPhoto = repositoryPhoto;
        this.env = env;
    }
    
    public async Task<List<Point>> GetAllPointsInEkaterinburg()
    {
        logger.LogInformation("{Method} called", nameof(GetAllPointsInEkaterinburg));
        
        try
        {
            var allPoints = await repositoryPoint.GetAllAsync();
            
            foreach (var point in allPoints)
            {
                point.Photos = await repositoryPhoto.GetAllInPointAsync(point.UId);
            }

            return allPoints;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Method}", nameof(GetAllPointsInEkaterinburg));
            throw;
        }
    }
    
    public async Task<Point?> GetPointById(uint id)
    {
        logger.LogInformation("{Method} called with id: {Id}", nameof(GetPointById), id);
        
        try
        {
            var point = await repositoryPoint.GetByIdAsync(id);
            if (point == null)
            {
                logger.LogWarning("{Method} point with id: {Id} not found", nameof(GetPointById), id);
                return null;
            }
            
            point.Photos = await repositoryPhoto.GetAllInPointAsync(point.UId);
            return point;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Method} with id: {Id}", nameof(GetPointById), id);
            throw;
        }
    }

    public async Task AddPoint(Point point)
    {
        logger.LogInformation("{Method} called with point: {PointId}", nameof(AddPoint), point.UId);
        
        try
        {
            await repositoryPoint.AddAsync(point);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Method}", nameof(AddPoint));
            throw;
        }
    }

    public async Task DeletePoint(uint id)
    {
        logger.LogInformation("{Method} called with id: {Id}", nameof(DeletePoint), id);
        
        try
        {
            var point = await repositoryPoint.GetByIdAsync(id);
            if (point == null)
            {
                logger.LogWarning("{Method} point with id: {Id} not found", nameof(DeletePoint), id);
                return;
            }

            await DeletePhotoFiles(point);
            await repositoryPoint.DeleteAsync(point);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Method} with id: {Id}", nameof(DeletePoint), id);
            throw;
        }
    }

    private async Task DeletePhotoFiles(Point point)
    {
        var photos = await repositoryPhoto.GetAllInPointAsync(point.UId);
        
        if (photos.Count == 0) return;

        var directoryPath = Path.Combine(env.WebRootPath, "uploads");
        
        if (!Directory.Exists(directoryPath))
        {
            logger.LogWarning("Directory not found: {DirectoryPath}", directoryPath);
            return;
        }

        foreach (var photo in photos)
        {
            try
            {
                var files = Directory.GetFiles(directoryPath, $"{photo.UId}.*");
                foreach (var file in files)
                {
                    File.Delete(file);
                    logger.LogInformation("Deleted file: {FilePath}", file);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting files for photo ID: {PhotoId}", photo.UId);
            }
        }
    }
}