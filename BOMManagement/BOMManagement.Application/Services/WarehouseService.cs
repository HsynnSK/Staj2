using AutoMapper;
using BOMManagement.Application.Common.Persistence;
using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.Warehouse;
using BOMManagement.Domain.Entities.Items;

namespace BOMManagement.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WarehouseService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IDataResult<IEnumerable<WarehouseDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var warehouses = await _unitOfWork.Repository<Warehouse>().GetAllAsync(cancellationToken);
        var dtos = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
        return new SuccessDataResult<IEnumerable<WarehouseDto>>(dtos);
    }
}
