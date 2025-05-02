using System.ComponentModel.DataAnnotations;

namespace PhotoMapAPI.Models;

public class Avatar
{
    [Key]
    public string UserId { get; private set; }
    public string AvatarPath { get; set; } // Путь в wwwroot/Avatars
    public readonly DateTime UploadedAt;

    public Avatar(string userId, string avatarPath)
    {
        UserId = userId;
        UploadedAt = DateTime.UtcNow;
    }
}