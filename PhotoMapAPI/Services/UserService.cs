using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhotoMapAPI.Models;

namespace PhotoMapAPI.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _userManager.Users
                .Include(u => u.UserAvatar)
                .Include(u => u.LikedPhoto)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userManager.Users
                .Include(u => u.UserAvatar)
                .ToListAsync();
        }
    }
}