using AutoMapper;
using BOMManagement.Application.Common.Exceptions;
using BOMManagement.Application.Common.Persistence;
using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.BOM;
using BOMManagement.Domain.Entities.BOMs;
using BOMManagement.Domain.Entities.Items;
using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.Services;

public class BOMService : IBOMService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BOMService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IDataResult<IEnumerable<BOMHeaderListDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var headers = await _unitOfWork.Repository<BOMHeader>().GetAllAsync(cancellationToken);
        var parentCodes = headers.Select(h => h.ParentItemCode).Distinct().ToList();
        var items = await _unitOfWork.Repository<Item>().FindAsync(i => parentCodes.Contains(i.ItemCode), cancellationToken);
        var itemDict = items.ToDictionary(i => i.ItemCode, i => i.ItemName);

        var dtos = _mapper.Map<IEnumerable<BOMHeaderListDto>>(headers).ToList();
        foreach (var dto in dtos)
        {
            if (itemDict.TryGetValue(dto.ParentItemCode, out var itemName))
            {
                dto.ParentItemName = itemName;
            }
        }

        return new SuccessDataResult<IEnumerable<BOMHeaderListDto>>(dtos);
    }

    public async Task<IDataResult<BOMHeaderDetailDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var header = await _unitOfWork.Repository<BOMHeader>().GetByIdWithIncludesAsync(id, cancellationToken, h => h.Lines!, h => h.Routes!);
        if (header == null)
        {
            throw new NotFoundException($"BOM with ID {id} was not found.");
        }

        var parentItem = (await _unitOfWork.Repository<Item>().FindAsync(i => i.ItemCode == header.ParentItemCode, cancellationToken)).FirstOrDefault();

        var childCodes = header.Lines?.Select(l => l.ChildItemCode).Distinct().ToList() ?? [];
        var childItems = await _unitOfWork.Repository<Item>().FindAsync(i => childCodes.Contains(i.ItemCode), cancellationToken);
        var childDict = childItems.ToDictionary(i => i.ItemCode, i => i.ItemName);

        var dto = _mapper.Map<BOMHeaderDetailDto>(header);
        if (parentItem != null)
        {
            dto.ParentItemName = parentItem.ItemName;
        }
        if (dto.Lines != null)
        {
            foreach (var line in dto.Lines)
            {
                if (childDict.TryGetValue(line.ChildItemCode, out var childName))
                {
                    line.ChildItemName = childName;
                }
            }
        }

        return new SuccessDataResult<BOMHeaderDetailDto>(dto);
    }

    public async Task<IResult> AddAsync(BOMHeaderCreateDto dto, CancellationToken cancellationToken = default)
    {
        // Business Rule: BOMCode must be unique
        var existingBOM = await _unitOfWork.Repository<BOMHeader>().FindAsync(x => x.BOMCode == dto.BOMCode, cancellationToken);
        if (existingBOM.Any())
        {
            throw new BusinessException($"A BOM with code '{dto.BOMCode}' already exists.");
        }

        // Business Rule: Parent item must exist and be FinishedGood or SubAssembly
        var parentItem = (await _unitOfWork.Repository<Item>().FindAsync(x => x.ItemCode == dto.ParentItemCode, cancellationToken)).FirstOrDefault();
        if (parentItem == null)
        {
            throw new NotFoundException($"Parent item with code '{dto.ParentItemCode}' was not found.");
        }
        if (parentItem.ItemType != ItemType.FinishedGood && parentItem.ItemType != ItemType.SubAssembly && parentItem.ItemType != ItemType.Subcontracted)
        {
            throw new BusinessException($"Parent item must be a Finished Good, Sub-Assembly or Subcontracted.");
        }

        // Validate child items
        foreach (var line in dto.Lines)
        {
            var childItem = (await _unitOfWork.Repository<Item>().FindAsync(x => x.ItemCode == line.ChildItemCode, cancellationToken)).FirstOrDefault();
            if (childItem == null)
            {
                throw new NotFoundException($"Child item with code '{line.ChildItemCode}' was not found.");
            }
        }

        // Deactivate other active BOMs for this parent item if the new one is active
        if (dto.IsActive)
        {
            await DeactivateOtherBOMsAsync(dto.ParentItemCode, 0, cancellationToken);
        }

        var header = _mapper.Map<BOMHeader>(dto);

        await _unitOfWork.Repository<BOMHeader>().AddAsync(header, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("BOM added successfully.");
    }

    public async Task<IResult> UpdateAsync(BOMHeaderUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var header = await _unitOfWork.Repository<BOMHeader>().GetByIdWithIncludesAsync(dto.Id, cancellationToken, h => h.Lines!, h => h.Routes!);
        if (header == null)
        {
            throw new NotFoundException($"BOM with ID {dto.Id} was not found.");
        }

        // Unique BOMCode
        if (header.BOMCode != dto.BOMCode)
        {
            var existingBOM = await _unitOfWork.Repository<BOMHeader>().FindAsync(x => x.BOMCode == dto.BOMCode, cancellationToken);
            if (existingBOM.Any())
            {
                throw new BusinessException($"A BOM with code '{dto.BOMCode}' already exists.");
            }
        }

        // Validate parent item
        var parentItem = (await _unitOfWork.Repository<Item>().FindAsync(x => x.ItemCode == dto.ParentItemCode, cancellationToken)).FirstOrDefault();
        if (parentItem == null)
        {
            throw new NotFoundException($"Parent item with code '{dto.ParentItemCode}' was not found.");
        }
        if (parentItem.ItemType != ItemType.FinishedGood && parentItem.ItemType != ItemType.SubAssembly && parentItem.ItemType != ItemType.Subcontracted)
        {
            throw new BusinessException($"Parent item must be a Finished Good, Sub-Assembly or Subcontracted.");
        }

        // Validate child items
        foreach (var line in dto.Lines)
        {
            var childItem = (await _unitOfWork.Repository<Item>().FindAsync(x => x.ItemCode == line.ChildItemCode, cancellationToken)).FirstOrDefault();
            if (childItem == null)
            {
                throw new NotFoundException($"Child item with code '{line.ChildItemCode}' was not found.");
            }
        }

        if (dto.IsActive)
        {
            await DeactivateOtherBOMsAsync(dto.ParentItemCode, dto.Id, cancellationToken);
        }

        // Clear existing children to avoid duplicates and map new ones cleanly
        if (header.Lines != null)
        {
            foreach (var line in header.Lines.ToList())
            {
                _unitOfWork.Repository<BOMLine>().Delete(line);
            }
        }
        if (header.Routes != null)
        {
            foreach (var route in header.Routes.ToList())
            {
                _unitOfWork.Repository<BOMRoute>().Delete(route);
            }
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _mapper.Map(dto, header);

        _unitOfWork.Repository<BOMHeader>().Update(header);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("BOM updated successfully.");
    }

    public async Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var header = await _unitOfWork.Repository<BOMHeader>().GetByIdAsync(id, cancellationToken);
        if (header == null)
        {
            throw new NotFoundException($"BOM with ID {id} was not found.");
        }

        _unitOfWork.Repository<BOMHeader>().Delete(header);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("BOM deleted successfully.");
    }

    public async Task<IResult> ToggleActiveStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var header = await _unitOfWork.Repository<BOMHeader>().GetByIdAsync(id, cancellationToken);
        if (header == null)
        {
            throw new NotFoundException($"BOM with ID {id} was not found.");
        }

        header.IsActive = !header.IsActive;

        if (header.IsActive)
        {
            await DeactivateOtherBOMsAsync(header.ParentItemCode, header.Id, cancellationToken);
        }

        _unitOfWork.Repository<BOMHeader>().Update(header);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult($"BOM state set to {(header.IsActive ? "Active" : "Inactive")}.");
    }

    private async Task DeactivateOtherBOMsAsync(string parentItemCode, int currentBomId, CancellationToken cancellationToken)
    {
        var otherActiveBOMs = await _unitOfWork.Repository<BOMHeader>().FindAsync(x => x.ParentItemCode == parentItemCode && x.IsActive && x.Id != currentBomId, cancellationToken);
        foreach (var bom in otherActiveBOMs)
        {
            bom.IsActive = false;
            _unitOfWork.Repository<BOMHeader>().Update(bom);
        }
    }

    public async Task<IDataResult<BOMTreeNodeDto>> GetBOMTreeAsync(string itemCode, decimal parentQty = 1, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.Repository<Item>().FindWithIncludesAsync(i => i.ItemCode == itemCode, cancellationToken, i => i.DefaultWarehouse!);
        var item = items.FirstOrDefault();
        if (item == null)
        {
            throw new NotFoundException($"Item with code '{itemCode}' was not found.");
        }

        var node = new BOMTreeNodeDto
        {
            ItemCode = item.ItemCode,
            ItemName = item.ItemName,
            Quantity = parentQty,
            Unit = GetUnitTypeDisplayName(item.MainUnit),
            ItemTypeDescription = GetItemTypeDisplayName(item.ItemType),
            IsSubAssembly = item.ItemType == ItemType.SubAssembly || item.ItemType == ItemType.Subcontracted,
            ConsumptionWarehouse = item.DefaultWarehouse?.WarehouseCode ?? string.Empty,
            StockQuantity = item.StockQuantity
        };

        if (item.ItemType == ItemType.SubAssembly || item.ItemType == ItemType.Subcontracted || item.ItemType == ItemType.FinishedGood)
        {
            var productions = await _unitOfWork.Repository<BOMManagement.Domain.Entities.Production.Production>().FindAsync(p => p.ItemCode == item.ItemCode, cancellationToken);
            node.ProductionNumbers = productions.Select(p => p.ProductionNo).ToList();
        }

        if (item.ItemType == ItemType.SubAssembly || item.ItemType == ItemType.FinishedGood || item.ItemType == ItemType.Subcontracted)
        {
            var boms = await _unitOfWork.Repository<BOMHeader>().FindWithIncludesAsync(b => b.ParentItemCode == itemCode && b.IsActive, cancellationToken, b => b.Lines!);
            var activeBOM = boms.FirstOrDefault();
            if (activeBOM != null && activeBOM.Lines != null)
            {
                foreach (var line in activeBOM.Lines)
                {
                    decimal requiredQty = (line.Quantity * parentQty) / activeBOM.BaseQuantity;
                    if (line.ScrapRate > 0)
                    {
                        requiredQty = requiredQty * (1 + line.ScrapRate / 100);
                    }

                    var childTreeResult = await GetBOMTreeAsync(line.ChildItemCode, requiredQty, cancellationToken);
                    if (childTreeResult.Success)
                    {
                        childTreeResult.Data.ConsumptionWarehouse = line.ConsumptionWarehouse;
                        node.Children.Add(childTreeResult.Data);
                    }
                }
            }
        }

        return new SuccessDataResult<BOMTreeNodeDto>(node);
    }

    private static string GetItemTypeDisplayName(ItemType type) => type switch
    {
        ItemType.RawMaterial => "Hammadde",
        ItemType.SubAssembly => "Yarı Mamül",
        ItemType.FinishedGood => "Mamül",
        ItemType.Subcontracted => "Fason",
        _ => type.ToString()
    };

    private static string GetUnitTypeDisplayName(UnitType unit) => unit switch
    {
        UnitType.Piece => "Adet",
        UnitType.Kg => "Kg",
        UnitType.Gram => "Gram",
        UnitType.Liter => "Litre",
        UnitType.Meter => "Metre",
        UnitType.SquareMeter => "m²",
        _ => unit.ToString()
    };

    public async Task<IDataResult<IEnumerable<BOMUsageDto>>> GetBOMUsagesByItemCodeAsync(string itemCode, CancellationToken cancellationToken = default)
    {
        var headers = await _unitOfWork.Repository<BOMHeader>().FindWithIncludesAsync(
            b => b.Lines!.Any(l => l.ChildItemCode == itemCode), 
            cancellationToken, 
            b => b.Lines!
        );

        var parentCodes = headers.Select(h => h.ParentItemCode).Distinct().ToList();
        var items = await _unitOfWork.Repository<Item>().FindAsync(i => parentCodes.Contains(i.ItemCode), cancellationToken);
        var itemDict = items.ToDictionary(i => i.ItemCode, i => i.ItemName);

        var usages = new List<BOMUsageDto>();
        foreach (var header in headers)
        {
            var line = header.Lines?.FirstOrDefault(l => l.ChildItemCode == itemCode);
            if (line != null)
            {
                itemDict.TryGetValue(header.ParentItemCode, out var parentName);
                usages.Add(new BOMUsageDto
                {
                    BOMHeaderId = header.Id,
                    BOMCode = header.BOMCode,
                    Version = header.Version,
                    ParentItemCode = header.ParentItemCode,
                    ParentItemName = parentName ?? string.Empty,
                    Quantity = line.Quantity,
                    UnitCode = line.UnitCode,
                    ScrapRate = line.ScrapRate,
                    ConsumptionWarehouse = line.ConsumptionWarehouse
                });
            }
        }

        return new SuccessDataResult<IEnumerable<BOMUsageDto>>(usages);
    }
}
