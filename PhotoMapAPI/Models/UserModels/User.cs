using Microsoft.AspNetCore.Identity;
using PhotoMapAPI.Models;

public class User : IdentityUser<string>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? UserAvatarId { get; set; }
    public virtual Avatar? UserAvatar { get; set; }

    // Лайкнутые фото
    public virtual ICollection<Photo> LikedPhotos { get; set; } = new List<Photo>();
}