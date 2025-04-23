namespace PhotoMapAPI.Models;

public class Avatar
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } // Связь с пользователем
    public string FilePath { get; set; } // Путь в wwwroot/Avatars
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}