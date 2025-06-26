namespace PhotoMapAPI.Controllers;
using System.Threading.Tasks;
using PhotoMapAPI.Models;

public interface IAuthService
{
    Task<string> GenerateJwtTokenAsync(User user);
}
