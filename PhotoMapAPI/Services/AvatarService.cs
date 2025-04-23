using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PhotoMapAPI.Models;
using System.IO;
using System.Threading.Tasks;
using PhotoMapAPI.Data;

public class AvatarService : IAvatarService
{
    private readonly IWebHostEnvironment _environment;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;

    public AvatarService(
        IWebHostEnvironment environment,
        UserManager<User> userManager,
        ApplicationDbContext context)
    {
        _environment = environment;
        _userManager = userManager;
        _context = context;
    }

    public async Task<string> UploadAvatarAsync(IFormFile file, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        // Удаляем старый аватар (если есть)
        await DeleteAvatarAsync(userId);

        // Создаем папку Avatars, если её нет
        var avatarsDir = Path.Combine(_environment.WebRootPath, "Avatars");
        if (!Directory.Exists(avatarsDir))
            Directory.CreateDirectory(avatarsDir);

        // Генерируем уникальное имя файла
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(avatarsDir, fileName);

        // Сохраняем файл
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Обновляем путь у пользователя
        user.AvatarPath = $"/Avatars/{fileName}";
        await _userManager.UpdateAsync(user);

        return user.AvatarPath;
    }

    public async Task<bool> DeleteAvatarAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user?.AvatarPath == null) return false;

        var filePath = Path.Combine(_environment.WebRootPath, user.AvatarPath.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        user.AvatarPath = null;
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<string?> GetAvatarPathAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.AvatarPath;
    }
}