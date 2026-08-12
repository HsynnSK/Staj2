using AutoMapper;
using BOMManagement.Application.Common.Persistence;
using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.Production;
using BOMManagement.Application.Common.Exceptions;
using BOMManagement.Application.Services;
using BOMManagement.Domain.Entities.BOMs;
using BOMManagement.Domain.Entities.Items;
using BOMManagement.Domain.Entities.Production;
using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.Services;

public class ProductionService : IProductionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductionService(IUnitOfWork unitOfWork, IMapper _mapper)
    {
        _unitOfWork = unitOfWork;
        this._mapper = _mapper;
    }

    public async Task<IDataResult<IEnumerable<ProductionListDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var productions = await _unitOfWork.Repository<Production>().GetAllAsync(cancellationToken);
        
        var dtoList = new List<ProductionListDto>();
        var items = await _unitOfWork.Repository<Item>().GetAllAsync(cancellationToken);
        var itemDict = items.ToDictionary(i => i.ItemCode, i => i.ItemName);

        foreach (var p in productions)
        {
            var dto = _mapper.Map<ProductionListDto>(p);
            if (itemDict.TryGetValue(p.ItemCode, out var itemName))
            {
                dto.ItemName = itemName;
            }
            dtoList.Add(dto);
        }

        return new SuccessDataResult<IEnumerable<ProductionListDto>>(dtoList.OrderByDescending(p => p.CreatedDate));
    }

    public async Task<IDataResult<ProductionDetailDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _unitOfWork.Repository<Production>().GetByIdAsync(id, cancellationToken);
        if (p == null)
        {
            throw new NotFoundException($"Production record with ID {id} was not found.");
        }

        var item = (await _unitOfWork.Repository<Item>().FindAsync(i => i.ItemCode == p.ItemCode, cancellationToken)).FirstOrDefault();

        var dto = _mapper.Map<ProductionDetailDto>(p);
        if (item != null)
        {
            dto.ItemName = item.ItemName;
        }

        return new SuccessDataResult<ProductionDetailDto>(dto);
    }

    public async Task<IResult> AddAsync(ProductionCreateDto dto, CancellationToken cancellationToken = default)
    {
        // Rule: ProductionNo must be unique
        var existing = await _unitOfWork.Repository<Production>().FindAsync(p => p.ProductionNo == dto.ProductionNo, cancellationToken);
        if (existing.Any())
        {
            throw new BusinessException($"Production with number '{dto.ProductionNo}' already exists.");
        }

        // Rule: ItemCode must exist and be SubAssembly or FinishedGood
        var item = (await _unitOfWork.Repository<Item>().FindAsync(i => i.ItemCode == dto.ItemCode, cancellationToken)).FirstOrDefault();
        if (item == null)
        {
            throw new NotFoundException($"Item with code '{dto.ItemCode}' was not found.");
        }
        if (item.ItemType != ItemType.SubAssembly && item.ItemType != ItemType.FinishedGood)
        {
            throw new BusinessException("Production can only be started for Finished Goods or Sub-Assemblies.");
        }

        // Rule: Must have an active BOM
        var activeBOM = (await _unitOfWork.Repository<BOMHeader>().FindAsync(b => b.ParentItemCode == dto.ItemCode && b.IsActive, cancellationToken)).FirstOrDefault();
        if (activeBOM == null)
        {
            throw new BusinessException($"Item '{dto.ItemCode}' does not have an active BOM recipe. Cannot start production.");
        }

        var p = _mapper.Map<Production>(dto);
        p.Status = ProductionStatus.Draft;
        p.BOMHeaderId = activeBOM.Id;

        await _unitOfWork.Repository<Production>().AddAsync(p, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Production created successfully as Draft.");
    }

    public async Task<IResult> UpdateStatusAsync(int id, ProductionStatus status, CancellationToken cancellationToken = default)
    {
        var p = await _unitOfWork.Repository<Production>().GetByIdAsync(id, cancellationToken);
        if (p == null)
        {
            throw new NotFoundException($"Production record with ID {id} was not found.");
        }

        // Prevent changes if completed or cancelled
        if (p.Status == ProductionStatus.Completed)
        {
            throw new BusinessException("Cannot change status of a completed Production.");
        }
        if (p.Status == ProductionStatus.Cancelled)
        {
            throw new BusinessException("Cannot change status of a cancelled Production.");
        }

        p.Status = status;

        // If Completed, execute actual stock movements
        if (status == ProductionStatus.Completed)
        {
            BOMHeader? activeBOM = null;
            if (p.BOMHeaderId.HasValue)
            {
                activeBOM = await _unitOfWork.Repository<BOMHeader>().GetByIdWithIncludesAsync(p.BOMHeaderId.Value, cancellationToken, h => h.Lines!);
            }
            else
            {
                var boms = await _unitOfWork.Repository<BOMHeader>().FindWithIncludesAsync(header => header.ParentItemCode == p.ItemCode && header.IsActive, cancellationToken, h => h.Lines!);
                activeBOM = boms.FirstOrDefault();
            }
            
            if (activeBOM != null && activeBOM.Lines != null)
            {
                // First Pass: Validate stock availability
                foreach (var line in activeBOM.Lines)
                {
                    decimal deductQty = (line.Quantity * p.TargetQuantity) / activeBOM.BaseQuantity;
                    if (line.ScrapRate > 0)
                    {
                        deductQty = deductQty * (1 + line.ScrapRate / 100);
                    }

                    var childItems = await _unitOfWork.Repository<Item>().FindAsync(i => i.ItemCode == line.ChildItemCode, cancellationToken);
                    var childItem = childItems.FirstOrDefault();
                    if (childItem == null || childItem.StockQuantity < deductQty)
                    {
                        var currentStock = childItem?.StockQuantity ?? 0;
                        throw new BusinessException($"Stok Yetersiz! '{line.ChildItemCode}' bileşeninden {deductQty:N2} adet gerekiyor, ancak eldeki stok {currentStock:N2} adet. Lütfen önce bu bileşeni üretin veya satın alın.");
                    }
                }

                // Second Pass: Deduct stocks
                foreach (var line in activeBOM.Lines)
                {
                    decimal deductQty = (line.Quantity * p.TargetQuantity) / activeBOM.BaseQuantity;
                    if (line.ScrapRate > 0)
                    {
                        deductQty = deductQty * (1 + line.ScrapRate / 100);
                    }

                    var childItems = await _unitOfWork.Repository<Item>().FindAsync(i => i.ItemCode == line.ChildItemCode, cancellationToken);
                    var childItem = childItems.FirstOrDefault();
                    if (childItem != null)
                    {
                        childItem.StockQuantity -= deductQty;
                        _unitOfWork.Repository<Item>().Update(childItem);
                    }
                }
            }

            var parentItems = await _unitOfWork.Repository<Item>().FindAsync(i => i.ItemCode == p.ItemCode, cancellationToken);
            var parentItem = parentItems.FirstOrDefault();
            if (parentItem != null)
            {
                parentItem.StockQuantity += p.TargetQuantity;
                _unitOfWork.Repository<Item>().Update(parentItem);
            }
        }

        _unitOfWork.Repository<Production>().Update(p);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult($"Production status updated to {status}.");
    }

    public async Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var p = await _unitOfWork.Repository<Production>().GetByIdAsync(id, cancellationToken);
        if (p == null)
        {
            throw new NotFoundException($"Production record with ID {id} was not found.");
        }

        // Only Draft productions can be deleted
        if (p.Status != ProductionStatus.Draft)
        {
            throw new BusinessException("Only Draft Production records can be deleted.");
        }

        _unitOfWork.Repository<Production>().Delete(p);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Production deleted successfully.");
    }
}
