using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhotoMapAPI.Models;

namespace PhotoMapAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;

        public UserService(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await userManager.Users
                .Include(u => u.UserAvatar)
                .Include(u => u.LikedPhotos)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await userManager.Users
                .Include(u => u.UserAvatar)
                .ToListAsync();
        }
        
        public async Task<List<string>> GetLikedPhotoIdsAsync(string userId)
        {
            var photos = await userManager.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.LikedPhotos)
                .Select(p => p.UId.ToString())
                .ToListAsync();

            return photos;
        }
    }
}