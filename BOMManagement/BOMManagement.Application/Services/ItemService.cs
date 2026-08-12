using AutoMapper;
using BOMManagement.Application.Common.Exceptions;
using BOMManagement.Application.Common.Persistence;
using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.Item;
using BOMManagement.Domain.Entities.Items;

namespace BOMManagement.Application.Services;

public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ItemService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IDataResult<IEnumerable<ItemListDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.Repository<Item>().GetAllWithIncludesAsync(cancellationToken, x => x.MaterialType, x => x.DefaultWarehouse!);
        var dtos = _mapper.Map<IEnumerable<ItemListDto>>(items);
        return new SuccessDataResult<IEnumerable<ItemListDto>>(dtos);
    }

    public async Task<IDataResult<ItemDetailDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.Repository<Item>().GetByIdWithIncludesAsync(id, cancellationToken, x => x.MaterialType, x => x.DefaultWarehouse!);
        if (item == null)
        {
            throw new NotFoundException($"Item with ID {id} was not found.");
        }

        var dto = _mapper.Map<ItemDetailDto>(item);
        return new SuccessDataResult<ItemDetailDto>(dto);
    }

    public async Task<IResult> AddAsync(ItemCreateDto dto, CancellationToken cancellationToken = default)
    {
        // Business Rule: ItemCode must be unique
        var existingItems = await _unitOfWork.Repository<Item>().FindAsync(x => x.ItemCode == dto.ItemCode, cancellationToken);
        if (existingItems.Any())
        {
            throw new BusinessException($"An item with code '{dto.ItemCode}' already exists.");
        }

        // Validate MaterialType exists
        var materialType = await _unitOfWork.Repository<MaterialType>().GetByIdAsync(dto.MaterialTypeId, cancellationToken);
        if (materialType == null)
        {
            throw new NotFoundException($"Material type with ID {dto.MaterialTypeId} was not found.");
        }

        // Validate DefaultWarehouse exists if specified
        Warehouse? defaultWarehouse = null;
        if (dto.DefaultWarehouseId.HasValue)
        {
            defaultWarehouse = await _unitOfWork.Repository<Warehouse>().GetByIdAsync(dto.DefaultWarehouseId.Value, cancellationToken);
            if (defaultWarehouse == null)
            {
                throw new NotFoundException($"Warehouse with ID {dto.DefaultWarehouseId.Value} was not found.");
            }
        }

        var item = _mapper.Map<Item>(dto);
        item.MaterialType = materialType; // EF Core maps shadow FK via navigation property
        item.DefaultWarehouse = defaultWarehouse;

        await _unitOfWork.Repository<Item>().AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Item added successfully.");
    }

    public async Task<IResult> UpdateAsync(ItemUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.Repository<Item>().GetByIdAsync(dto.Id, cancellationToken);
        if (item == null)
        {
            throw new NotFoundException($"Item with ID {dto.Id} was not found.");
        }

        // Business Rule: ItemCode must be unique if changed
        if (item.ItemCode != dto.ItemCode)
        {
            var existingItems = await _unitOfWork.Repository<Item>().FindAsync(x => x.ItemCode == dto.ItemCode, cancellationToken);
            if (existingItems.Any())
            {
                throw new BusinessException($"An item with code '{dto.ItemCode}' already exists.");
            }
        }

        // Validate MaterialType exists
        var materialType = await _unitOfWork.Repository<MaterialType>().GetByIdAsync(dto.MaterialTypeId, cancellationToken);
        if (materialType == null)
        {
            throw new NotFoundException($"Material type with ID {dto.MaterialTypeId} was not found.");
        }

        // Validate DefaultWarehouse exists if specified
        Warehouse? defaultWarehouse = null;
        if (dto.DefaultWarehouseId.HasValue)
        {
            defaultWarehouse = await _unitOfWork.Repository<Warehouse>().GetByIdAsync(dto.DefaultWarehouseId.Value, cancellationToken);
            if (defaultWarehouse == null)
            {
                throw new NotFoundException($"Warehouse with ID {dto.DefaultWarehouseId.Value} was not found.");
            }
        }

        _mapper.Map(dto, item);
        item.MaterialType = materialType; // EF Core maps shadow FK via navigation property
        item.DefaultWarehouse = defaultWarehouse;

        _unitOfWork.Repository<Item>().Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Item updated successfully.");
    }

    public async Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.Repository<Item>().GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            throw new NotFoundException($"Item with ID {id} was not found.");
        }

        _unitOfWork.Repository<Item>().Delete(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Item deleted successfully.");
    }
}
