using AutoMapper;
using BOMManagement.Application.Common.Persistence;
using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.MaterialType;
using BOMManagement.Domain.Entities.Items;

namespace BOMManagement.Application.Services;

public class MaterialTypeService : IMaterialTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MaterialTypeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IDataResult<IEnumerable<MaterialTypeDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var materialTypes = await _unitOfWork.Repository<MaterialType>().GetAllAsync(cancellationToken);
        var dtos = _mapper.Map<IEnumerable<MaterialTypeDto>>(materialTypes);
        return new SuccessDataResult<IEnumerable<MaterialTypeDto>>(dtos);
    }
}
