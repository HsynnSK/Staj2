using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.Item;

namespace BOMManagement.Application.Services;

public interface IItemService
{
    Task<IDataResult<IEnumerable<ItemListDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<ItemDetailDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IResult> AddAsync(ItemCreateDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(ItemUpdateDto dto, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
