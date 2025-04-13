using PhotoMapAPI.Controllers;
using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;

namespace PhotoMapAPI.Services;

public class PhotoServices : IPhotoServices
{
    private readonly IPhotoRepository repository;
    private readonly IWebHostEnvironment env;
    private readonly ILogger<PhotoServices> logger;
    public PhotoServices(IPhotoRepository repository, IWebHostEnvironment env, ILogger<PhotoServices> logger)
    {
        this.env = env;
        this.logger = logger;
        this.repository = repository;
    }
    public async Task<Photo?> GetPhotoById(uint id)
    {
        logger.Log(LogLevel.Information,$"{nameof(GetPhotoById)} called with id: {id}");
        return await repository.GetByIdAsync(id);
    }
    public async Task DeletePhoto(uint id)
    {
        logger.LogInformation("{Method} called with id: {Id}", nameof(DeletePhoto), id);

        var photo = await repository.GetByIdAsync(id);
        if (photo == null)
        {
            logger.LogWarning("Photo with ID {Id} not found.", id);
            throw new KeyNotFoundException($"Photo with ID {id} not found.");
        }

        var filePath = Path.Combine(env.WebRootPath, "uploads", $"{photo.UId}.jpg");
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                logger.LogInformation("Deleted file at path: {Path}", filePath);
            }
            else
            {
                logger.LogWarning("File at path {Path} does not exist.", filePath);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting file at path: {Path}", filePath);
            throw;
        }

        await repository.DeleteAsync(photo);
        logger.LogInformation("Photo with ID {Id} deleted from repository.", id);
    }
}