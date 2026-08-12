using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.BOM;

namespace BOMManagement.Application.Services;

public interface IBOMService
{
    Task<IDataResult<IEnumerable<BOMHeaderListDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<BOMHeaderDetailDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IResult> AddAsync(BOMHeaderCreateDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(BOMHeaderUpdateDto dto, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IResult> ToggleActiveStatusAsync(int id, CancellationToken cancellationToken = default);
    Task<IDataResult<BOMTreeNodeDto>> GetBOMTreeAsync(string itemCode, decimal parentQty = 1, CancellationToken cancellationToken = default);
    Task<IDataResult<IEnumerable<BOMUsageDto>>> GetBOMUsagesByItemCodeAsync(string itemCode, CancellationToken cancellationToken = default);
}
