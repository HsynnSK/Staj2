using AutoMapper;
using BOMManagement.Application.Common.Persistence;
using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.WorkOrder;
using BOMManagement.Application.Common.Exceptions;
using BOMManagement.Application.Services;
using BOMManagement.Domain.Entities.Items;
using BOMManagement.Domain.Entities.Production;

namespace BOMManagement.Application.Services;

public class WorkOrderService : IWorkOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WorkOrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IDataResult<IEnumerable<WorkOrderListDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var workOrders = await _unitOfWork.Repository<WorkOrder>().GetAllAsync(cancellationToken);
        var dtos = _mapper.Map<IEnumerable<WorkOrderListDto>>(workOrders);
        return new SuccessDataResult<IEnumerable<WorkOrderListDto>>(dtos);
    }

    public async Task<IDataResult<IEnumerable<WorkOrderListDto>>> GetByItemCodeAsync(string itemCode, CancellationToken cancellationToken = default)
    {
        var workOrders = await _unitOfWork.Repository<WorkOrder>()
            .FindAsync(w => w.ItemCode == itemCode, cancellationToken);
        var dtos = _mapper.Map<IEnumerable<WorkOrderListDto>>(workOrders);
        return new SuccessDataResult<IEnumerable<WorkOrderListDto>>(dtos);
    }

    public async Task<IDataResult<WorkOrderDetailDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var workOrder = await _unitOfWork.Repository<WorkOrder>().GetByIdAsync(id, cancellationToken);
        if (workOrder == null)
        {
            throw new NotFoundException($"İş Emri bulunamadı (ID: {id}).");
        }
        var dto = _mapper.Map<WorkOrderDetailDto>(workOrder);
        return new SuccessDataResult<WorkOrderDetailDto>(dto);
    }

    public async Task<IResult> AddAsync(WorkOrderCreateDto dto, CancellationToken cancellationToken = default)
    {
        // Check unique index on ItemCode
        var existingForItem = await _unitOfWork.Repository<WorkOrder>().FindAsync(w => w.ItemCode == dto.ItemCode, cancellationToken);
        if (existingForItem.Any())
        {
            throw new BusinessException($"'{dto.ItemCode}' kodlu malzemenin zaten bir iş emri bulunuyor.");
        }

        // Validate ItemCode
        var itemResult = await _unitOfWork.Repository<Item>().FindAsync(i => i.ItemCode == dto.ItemCode, cancellationToken);
        if (!itemResult.Any())
        {
            throw new NotFoundException($"'{dto.ItemCode}' kodlu malzeme bulunamadı.");
        }

        var workOrder = _mapper.Map<WorkOrder>(dto);
        await _unitOfWork.Repository<WorkOrder>().AddAsync(workOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("İş emri başarıyla oluşturuldu.");
    }

    public async Task<IResult> UpdateAsync(WorkOrderUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var workOrder = await _unitOfWork.Repository<WorkOrder>().GetByIdAsync(dto.Id, cancellationToken);
        if (workOrder == null)
        {
            throw new NotFoundException($"Güncellenecek iş emri bulunamadı (ID: {dto.Id}).");
        }

        _mapper.Map(dto, workOrder);
        _unitOfWork.Repository<WorkOrder>().Update(workOrder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("İş emri başarıyla güncellendi.");
    }

    public async Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var workOrder = await _unitOfWork.Repository<WorkOrder>().GetByIdAsync(id, cancellationToken);
        if (workOrder == null)
        {
            throw new NotFoundException($"Silinecek iş emri bulunamadı (ID: {id}).");
        }

        _unitOfWork.Repository<WorkOrder>().Delete(workOrder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("İş emri silindi.");
    }
}
