using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using PhotoMapAPI.Models;

public class User : IdentityUser<string>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public List<Photo> LikedPhoto { get; set; } = new();

    public string? UserAvatarId { get; set; }
    public virtual Avatar? UserAvatar { get; set; }
}