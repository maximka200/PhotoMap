using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoMapAPI.Models;

namespace PhotoMapAPI.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(string id);
        Task<List<User>> GetAllUsersAsync();
        Task<List<string>> GetLikedPhotoIdsAsync(string userId);
    }
}