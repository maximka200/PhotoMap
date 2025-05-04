using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoMapAPI.Models;

public class Avatar
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string AvatarPath { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }

    public Avatar(string userId, string avatarPath)
    {
        UserId = userId;
        AvatarPath = avatarPath;
    }

    public Avatar() { }
}