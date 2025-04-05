using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhotoMapAPI.Controllers;
using PhotoMapAPI.Data;
using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;

namespace PhotoMapAPI.Services;

public class PhotoUploadServices : IPhotoUploadServices
{
    private readonly IRepository<Point> pointRepository;
    private readonly IRepository<Photo> photoRepository;
    private readonly ApplicationDbContext dbContext;
    private readonly IWebHostEnvironment env;
    private readonly ILogger<PhotoUploadServices> logger;

    public PhotoUploadServices(IRepository<Point> pointRepository, IRepository<Photo> photoRepository,
                        ApplicationDbContext dbContext, IWebHostEnvironment env, ILogger<PhotoUploadServices> logger)
    {
        this.pointRepository = pointRepository;
        this.photoRepository = photoRepository;
        this.dbContext = dbContext;
        this.env = env;
        this.logger = logger;
    }

    public async Task<string> UploadPhotoAsync(IFormFile file, uint pointId)
    {
        logger.Log(LogLevel.Information, $"{nameof(UploadPhotoAsync)} called with pointId: {pointId}");
        if (file == null || file.Length == 0)
            throw new ArgumentException("Файл не загружен.");

        var point = await pointRepository.GetByIdAsync(pointId);
        if (point == null)
            throw new KeyNotFoundException("Точка не найдена.");

        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var uploadsFolder = Path.Combine(env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            // Создаём запись в БД без URL, чтобы получить UId
            var newPhoto = new Photo("", pointId);
            point.AddPhoto(newPhoto);
            await photoRepository.AddAsync(newPhoto);
            await dbContext.SaveChangesAsync();
            
            // Определяем расширение файла
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{newPhoto.UId}{fileExtension}"; // Пример: 1.jpg, 2.png
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Сохраняем файл
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Обновляем URL и сохраняем изменения
            newPhoto.Url = $"/uploads/{fileName}";
            await photoRepository.UpdateAsync(newPhoto);
            await dbContext.SaveChangesAsync();

            await transaction.CommitAsync(); // Завершаем транзакцию
            return newPhoto.Url;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(); // Откат всех изменений, если ошибка
            logger.LogError(ex, "Ошибка при загрузке фото");
            throw new Exception("Ошибка при сохранении фото.");
        }
    }
}
