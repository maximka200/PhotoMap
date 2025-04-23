using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<string>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public List<Photo> LikedPhoto { get; set; } = new();

    public string? AvatarPath { get; set; } // Путь к аватарке
}