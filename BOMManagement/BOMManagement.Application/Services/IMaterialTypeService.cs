using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.MaterialType;

namespace BOMManagement.Application.Services;

public interface IMaterialTypeService
{
    Task<IDataResult<IEnumerable<MaterialTypeDto>>> GetAllAsync(CancellationToken cancellationToken = default);
}
