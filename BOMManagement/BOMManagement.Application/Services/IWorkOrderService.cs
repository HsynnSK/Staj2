using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.WorkOrder;

namespace BOMManagement.Application.Services;

public interface IWorkOrderService
{
    Task<IDataResult<IEnumerable<WorkOrderListDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<IEnumerable<WorkOrderListDto>>> GetByItemCodeAsync(string itemCode, CancellationToken cancellationToken = default);
    Task<IDataResult<WorkOrderDetailDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IResult> AddAsync(WorkOrderCreateDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(WorkOrderUpdateDto dto, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
