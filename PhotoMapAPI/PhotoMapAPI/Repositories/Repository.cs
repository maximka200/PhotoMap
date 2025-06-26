using Microsoft.EntityFrameworkCore;
using PhotoMapAPI.Data;
using PhotoMapAPI.Services;

namespace PhotoMapAPI.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext context;
    protected readonly DbSet<T> dbSet;
    protected readonly ILogger<Repository<T>> logger;
    
    public Repository(ApplicationDbContext context, ILogger<Repository<T>> logger)
    {
        this.context = context;
        this.logger = logger;
        dbSet = context.Set<T>();
    }
    
    public async Task AddAsync(T entity)
    {
        try
        {
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {ex}");
        }
    }

    public async Task DeleteAsync(T entity)
    {
        try
        {
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {ex}");
        }
    }

    public async Task<T?> GetByIdAsync(uint id)
    {
        try
        {
            return await dbSet.FindAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {ex}");
            return null;
        }
    }

    public async Task UpdateAsync(T entity)
    {
        try
        {
            dbSet.Update(entity);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {ex}");
        }
    }
}