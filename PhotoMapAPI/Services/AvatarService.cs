using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PhotoMapAPI.Data;
using PhotoMapAPI.Models;

namespace PhotoMapAPI.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly IWebHostEnvironment env;
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<AvatarService> logger;

        public AvatarService(IWebHostEnvironment env, UserManager<User> userManager,
                              ApplicationDbContext dbContext, ILogger<AvatarService> logger)
        {
            this.env = env;
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<string> UploadAvatarAsync(IFormFile file, string userId)
        {
            logger.LogInformation($"{nameof(UploadAvatarAsync)} called with userId: {userId}");
            if (file == null || file.Length == 0)
                throw new ArgumentException("File not uploaded.");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var avatarsFolder = Path.Combine(env.WebRootPath, "avatars");
                Directory.CreateDirectory(avatarsFolder);

                if (!(user.UserAvatar == null || string.IsNullOrEmpty(user.UserAvatar.AvatarPath)))
                {
                    var oldAvatarPath = Path.Combine(env.WebRootPath, user.UserAvatar.AvatarPath.TrimStart('/'));
                    if (File.Exists(oldAvatarPath))
                    {
                        File.Delete(oldAvatarPath);
                    }
                }
                
                user.UserAvatar = new Avatar(userId, null);
                
                var extension = Path.GetExtension(file.FileName);
                var fileName = user.Id + extension;
                var filePath = Path.Combine(avatarsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                user.UserAvatar.AvatarPath = $"/avatars/{fileName}";
                await userManager.UpdateAsync(user);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return user.UserAvatar.AvatarPath;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Error uploading avatar");
                throw new Exception("Error uploading avatar", ex);
            }
        }

        public async Task<bool> DeleteAvatarAsync(string userId)
        {
            logger.LogInformation($"{nameof(DeleteAvatarAsync)} called with userId: {userId}");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.UserAvatar.AvatarPath))
                return false;

            var avatarPath = Path.Combine(env.WebRootPath, user.UserAvatar.AvatarPath.TrimStart('/'));
            if (File.Exists(avatarPath))
            {
                File.Delete(avatarPath);
            }

            user.UserAvatar.AvatarPath = null;
            await userManager.UpdateAsync(user);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<string?> GetAvatarPathAsync(string userId)
        {
            logger.LogInformation($"{nameof(GetAvatarPathAsync)} called with userId: {userId}");

            var user = await userManager.FindByIdAsync(userId);
            return user?.UserAvatar.AvatarPath;
        }
    }
}
