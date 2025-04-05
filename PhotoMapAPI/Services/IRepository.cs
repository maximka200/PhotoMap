namespace PhotoMapAPI.Services;

public interface IRepository<T>
{
    Task AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T?> GetByIdAsync(uint id);
    Task UpdateAsync(T entity);
}