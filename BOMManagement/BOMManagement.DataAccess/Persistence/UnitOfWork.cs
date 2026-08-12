using System.Collections.Concurrent;
using BOMManagement.Application.Common.Persistence;
using BOMManagement.Domain.Entities.BaseModels;

namespace BOMManagement.DataAccess.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly BOMDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories;
    private bool _disposed;

    public UnitOfWork(BOMDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _repositories = new ConcurrentDictionary<Type, object>();
    }

    public IRepository<T> Repository<T>() where T : DatabaseObject
    {
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new Repository<T>(_context));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
