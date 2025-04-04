using PhotoMapAPI.Controllers;
using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;

namespace PhotoMapAPI.Services;

public class PhotoServices : IPhotoServices
{
    private readonly IRepository<Photo> repository;
    private readonly IWebHostEnvironment env;
    private readonly ILogger<PhotoServices> logger;
    public PhotoServices(IRepository<Photo> repository, IWebHostEnvironment env, ILogger<PhotoServices> logger)
    {
        this.env = env;
        this.logger = logger;
        this.repository = repository;
    }
    public async Task<Photo?> GetPhotoById(uint id)
    {
        return await repository.GetByIdAsync(id);
    }
}