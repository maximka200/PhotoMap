using PhotoMapAPI.Controllers;
using PhotoMapAPI.Models;
using PhotoMapAPI.Repositories;
using Point = System.Drawing.Point;

namespace PhotoMapAPI.Services;

public class PhotoServices : IPhotoServices
{
    private readonly Repository<Photo> repository;
    public async Task<Photo?> GetPhotoById(uint id)
    {
        return await repository.GetByIdAsync(id);
    }
}