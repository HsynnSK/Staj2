using AutoMapper;
using BOMManagement.Application.Common.Exceptions;
using BOMManagement.Application.Common.Persistence;
using BOMManagement.Application.Common.Results;
using BOMManagement.Application.DTOs.PurchaseOrder;
using BOMManagement.Domain.Entities.Items;
using BOMManagement.Domain.Entities.Production;
using BOMManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BOMManagement.Application.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PurchaseOrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IDataResult<IEnumerable<PurchaseOrderListDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var pos = await _unitOfWork.Repository<PurchaseOrder>().GetAllAsync(cancellationToken);
        var itemCodes = pos.Select(p => p.ItemCode).Distinct().ToList();
        var items = await _unitOfWork.Repository<Item>().FindAsync(i => itemCodes.Contains(i.ItemCode), cancellationToken);
        var itemDict = items.ToDictionary(i => i.ItemCode, i => i.ItemName);

        var dtos = _mapper.Map<IEnumerable<PurchaseOrderListDto>>(pos).ToList();
        foreach (var dto in dtos)
        {
            if (itemDict.TryGetValue(dto.ItemCode, out var itemName))
            {
                dto.ItemName = itemName;
            }
        }

        return new SuccessDataResult<IEnumerable<PurchaseOrderListDto>>(dtos.OrderByDescending(p => p.CreatedDate));
    }

    public async Task<IResult> AddAsync(PurchaseOrderCreateDto dto, CancellationToken cancellationToken = default)
    {
        // Rule: PurchaseOrderNo must be unique
        var existing = await _unitOfWork.Repository<PurchaseOrder>().FindAsync(p => p.PurchaseOrderNo == dto.PurchaseOrderNo, cancellationToken);
        if (existing.Any())
        {
            throw new BusinessException($"Purchase Order with number '{dto.PurchaseOrderNo}' already exists.");
        }

        // Rule: ItemCode must exist
        var item = (await _unitOfWork.Repository<Item>().FindAsync(i => i.ItemCode == dto.ItemCode, cancellationToken)).FirstOrDefault();
        if (item == null)
        {
            throw new NotFoundException($"Item with code '{dto.ItemCode}' was not found.");
        }

        var po = _mapper.Map<PurchaseOrder>(dto);
        po.Status = PurchaseOrderStatus.Draft;

        await _unitOfWork.Repository<PurchaseOrder>().AddAsync(po, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Purchase Order created successfully as Draft.");
    }

    public async Task<IResult> UpdateStatusAsync(int id, PurchaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        var purchaseOrder = await _unitOfWork.Repository<PurchaseOrder>().GetByIdAsync(id, cancellationToken);
        if (purchaseOrder == null)
        {
            throw new NotFoundException($"Purchase Order with ID {id} was not found.");
        }

        // Prevent changes if delivered or cancelled
        if (purchaseOrder.Status == PurchaseOrderStatus.Delivered)
        {
            throw new BusinessException("Cannot change status of a delivered Purchase Order.");
        }
        if (purchaseOrder.Status == PurchaseOrderStatus.Cancelled)
        {
            throw new BusinessException("Cannot change status of a cancelled Purchase Order.");
        }

        purchaseOrder.Status = status;

        // If Delivered, increase stock levels
        if (status == PurchaseOrderStatus.Delivered)
        {
            var items = await _unitOfWork.Repository<Item>().FindAsync(i => i.ItemCode == purchaseOrder.ItemCode, cancellationToken);
            var item = items.FirstOrDefault();
            if (item != null)
            {
                item.StockQuantity += purchaseOrder.Quantity;
                _unitOfWork.Repository<Item>().Update(item);
            }
        }

        _unitOfWork.Repository<PurchaseOrder>().Update(purchaseOrder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult($"Purchase Order status updated to {status}.");
    }
}
