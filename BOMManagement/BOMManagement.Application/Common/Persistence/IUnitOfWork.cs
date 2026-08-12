using BOMManagement.Domain.Entities.BaseModels;

namespace BOMManagement.Application.Common.Persistence;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : DatabaseObject;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
