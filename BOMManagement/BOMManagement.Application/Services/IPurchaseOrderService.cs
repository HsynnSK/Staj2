using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.PurchaseOrder;
using BOMManagement.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BOMManagement.Application.Services;

public interface IPurchaseOrderService
{
    Task<IDataResult<IEnumerable<PurchaseOrderListDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IResult> AddAsync(PurchaseOrderCreateDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateStatusAsync(int id, PurchaseOrderStatus status, CancellationToken cancellationToken = default);
}
