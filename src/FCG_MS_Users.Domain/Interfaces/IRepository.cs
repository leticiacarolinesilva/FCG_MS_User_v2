using System.Linq.Expressions;

namespace FCG_MS_Users.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync<T>(Guid id);
    Task<IEnumerable<T>> GetAllAsync<T>();
    Task<IEnumerable<T>> FindAsync<T>(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}