using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.Production;
using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.Services;

public interface IProductionService
{
    Task<IDataResult<IEnumerable<ProductionListDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<ProductionDetailDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IResult> AddAsync(ProductionCreateDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateStatusAsync(int id, ProductionStatus status, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
