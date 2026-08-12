using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.Warehouse;

namespace BOMManagement.Application.Services;

public interface IWarehouseService
{
    Task<IDataResult<IEnumerable<WarehouseDto>>> GetAllAsync(CancellationToken cancellationToken = default);
}
