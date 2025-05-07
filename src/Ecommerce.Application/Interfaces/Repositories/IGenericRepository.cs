namespace Ecommerce.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(int pageIndex, int pageSize);
    Task<T?> GetByIdAsync(Guid id);
    Task<int> AddAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(Guid id);
    Task<int> CountAsync();
    Task<IEnumerable<T>> GetByIdsAsync(List<Guid> ids);
} 
