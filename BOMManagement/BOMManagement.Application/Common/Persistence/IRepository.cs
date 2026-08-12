using System.Linq.Expressions;
using BOMManagement.Domain.Entities.BaseModels;

namespace BOMManagement.Application.Common.Persistence;

public interface IRepository<T> where T : DatabaseObject
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<T?> GetByIdWithIncludesAsync(int id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
