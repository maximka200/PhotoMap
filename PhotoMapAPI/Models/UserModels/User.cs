using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using PhotoMapAPI.Models;

public class User : IdentityUser<string>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public List<Photo> LikedPhoto { get; set; } = new();

    public Avatar? UserAvatar { get; set; } // Путь к аватарке
}