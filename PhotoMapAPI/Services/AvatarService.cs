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

                var extension = Path.GetExtension(file.FileName);
                var fileName = user.Id + extension;
                var filePath = Path.Combine(avatarsFolder, fileName);
                
                var avatarPath = $"/avatars/{fileName}";
                var avatar = await dbContext.Avatars.FindAsync(user.Id);
                if (avatar != null)
                {
                    var oldPath = Path.Combine(env.WebRootPath, avatar.AvatarPath.TrimStart('/'));
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);

                    avatar.AvatarPath = avatarPath;
                    dbContext.Avatars.Update(avatar);
                }
                else
                {
                    avatar = new Avatar(user.Id, avatarPath);
                    dbContext.Avatars.Add(avatar);
                }
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                user.UserAvatarId = user.Id; // Привязка один к одному
                await userManager.UpdateAsync(user);
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return avatarPath;
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
            if (user == null || user.UserAvatarId == null)
                return false;

            var avatar = await dbContext.Avatars.FindAsync(user.UserAvatarId);
            if (avatar == null)
                return false;

            var avatarPath = Path.Combine(env.WebRootPath, avatar.AvatarPath.TrimStart('/'));
            if (File.Exists(avatarPath))
            {
                File.Delete(avatarPath);
            }

            dbContext.Avatars.Remove(avatar);
            user.UserAvatarId = null;

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
