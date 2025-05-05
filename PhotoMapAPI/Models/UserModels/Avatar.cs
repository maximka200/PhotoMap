using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoMapAPI.Models;

public class Avatar
{
    [Key, ForeignKey("User")]
    public string UserId { get; set; }

    [Required]
    public string AvatarPath { get; set; }

    public virtual User User { get; set; }

    public Avatar() { }

    public Avatar(string userId, string avatarPath)
    {
        UserId = userId;
        AvatarPath = avatarPath;
    }
}