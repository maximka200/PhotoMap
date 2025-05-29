using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhotoMapAPI.Models;

namespace PhotoMapAPI.Services
{
    public class UserService
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
    }
}