using System.Linq.Expressions;

namespace Application.Database;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();

    Task<T?> GetByIdAsync(int id);

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);

    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
}
