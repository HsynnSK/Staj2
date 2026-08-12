using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OrderSystemManagment.Domain.Entities;
using OrderSystemManagment.Domain.Entities.BaseModels;
using OrderSystemManagment.Domain.Interfaces;
using OrderSystemManagment.DataAccess.Context;
using System.Linq;

namespace OrderSystemManagment.DataAccess.Repositories;

/// <summary>
/// Pure persistence repository. Contains no business logic.
/// All orchestration (currency conversion, invoice generation) lives in OrderService.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "OrderItemToOrderMap";

    public OrderRepository(OrderDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task SaveOrderAsync(Order order)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        if (order.Customer is not null)
            order.Customer = AttachOrGetTracked(order.Customer);

        if (order.CurrencyType is not null)
        {
            order.CurrencyType = AttachOrGetTracked(order.CurrencyType);
            if (order.Invoice is not null)
                order.Invoice.CurrencyType = order.CurrencyType;
        }

        foreach (var orderItem in order.OrderItems)
        {
            orderItem.Order = order;

            if (orderItem.Item is not null)
            {
                if (orderItem.Item.Warehouse is not null)
                    orderItem.Item.Warehouse = AttachOrGetTracked(orderItem.Item.Warehouse);

                orderItem.Item = AttachOrGetTracked(orderItem.Item);
            }

            if (orderItem.CurrencyType is not null)
                orderItem.CurrencyType = AttachOrGetTracked(orderItem.CurrencyType);
        }

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        _cache.Remove(CacheKey);
    }

    public async Task UpdateOrderAsync(Order order)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var existingOrder = await _context.Orders
            .Include(x => x.Invoice)
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == order.Id);

        if (existingOrder is null)
            throw new InvalidOperationException($"Order with id {order.Id} was not found.");

        existingOrder.PaymentDate = order.PaymentDate;

        if (order.Customer is not null)
            existingOrder.Customer = AttachOrGetTracked(order.Customer);
        else
            existingOrder.Customer = null;

        if (order.CurrencyType is not null)
            existingOrder.CurrencyType = AttachOrGetTracked(order.CurrencyType);
        else
            existingOrder.CurrencyType = null;

        if (existingOrder.Invoice is null)
        {
            existingOrder.Invoice = new Invoice
            {
                InvoiceNumber = string.Empty,
                InvoiceType = string.Empty,
                InvoiceDate = DateTime.Now
            };
        }

        // Copy the pre-computed invoice fields from the incoming order's Invoice
        if (order.Invoice is not null)
        {
            existingOrder.Invoice.CurrencyType = existingOrder.CurrencyType;
            existingOrder.Invoice.GrandTotal = order.Invoice.GrandTotal;
            existingOrder.Invoice.ExchangeRate = order.Invoice.ExchangeRate;
            existingOrder.Invoice.ExchangeRateDate = order.Invoice.ExchangeRateDate;
        }

        foreach (var oldItem in existingOrder.OrderItems.ToList())
            oldItem.IsDeleted = true;

        foreach (var orderItem in order.OrderItems)
        {
            var newOrderItem = new OrderItem
            {
                Order = existingOrder,
                UnitPrice = orderItem.UnitPrice,
                Quantity = orderItem.Quantity
            };

            if (orderItem.Item is not null)
            {
                if (orderItem.Item.Warehouse is not null)
                    orderItem.Item.Warehouse = AttachOrGetTracked(orderItem.Item.Warehouse);

                newOrderItem.Item = AttachOrGetTracked(orderItem.Item);
            }

            if (orderItem.CurrencyType is not null)
                newOrderItem.CurrencyType = AttachOrGetTracked(orderItem.CurrencyType);

            existingOrder.OrderItems.Add(newOrderItem);
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        _cache.Remove(CacheKey);
    }

    public async Task SoftDeleteOrderAsync(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var order = await _context.Orders
            .Include(x => x.Invoice)
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order is null)
            return;

        order.IsDeleted = true;

        if (order.Invoice is not null)
            order.Invoice.IsDeleted = true;

        foreach (var orderItem in order.OrderItems)
            orderItem.IsDeleted = true;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        _cache.Remove(CacheKey);
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.CurrencyType)
            .Include(x => x.Invoice)
                .ThenInclude(x => x!.CurrencyType)
            .Include(x => x.OrderItems)
                .ThenInclude(x => x.Item!)
                    .ThenInclude(x => x.Warehouse)
            .Include(x => x.OrderItems)
                .ThenInclude(x => x.CurrencyType)
            .OrderByDescending(x => x.Invoice!.InvoiceDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.CurrencyType)
            .Include(x => x.Invoice)
                .ThenInclude(x => x!.CurrencyType)
            .Include(x => x.OrderItems)
                .ThenInclude(x => x.Item!)
                    .ThenInclude(x => x.Warehouse)
            .Include(x => x.OrderItems)
                .ThenInclude(x => x.CurrencyType)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var year = DateTime.Now.Year;
        var prefix = $"INV{year}";

        var existingNumbers = await _context.Invoices
            .AsNoTracking()
            .Where(x => x.InvoiceNumber.StartsWith(prefix))
            .Select(x => x.InvoiceNumber)
            .ToListAsync();

        var nextSequence = existingNumbers
            .Select(x => x.Length >= prefix.Length + 7 && int.TryParse(x.Substring(prefix.Length, 7), out var number)
                ? number
                : 0)
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"{prefix}{nextSequence:D7}";
    }

    public async Task<int?> GetOrderIdByOrderItemIdAsync(int orderItemId)
    {
        var orderItemToOrderMap = await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            return await _context.OrderItems
                .Select(x => new
                {
                    OrderItemId = x.Id,
                    OrderId = EF.Property<int>(x, "OrderId")
                })
                .ToDictionaryAsync(k => k.OrderItemId, v => v.OrderId);
        });

        if (orderItemToOrderMap != null && orderItemToOrderMap.TryGetValue(orderItemId, out int bulunanOrderId))
        {
            return bulunanOrderId;
        }

        return null;
    }



    private List<Order> orders = new List<Order>();

    public List<Order> GetTest()
    {



        return orders;

    }

    private TEntity AttachOrGetTracked<TEntity>(TEntity entity) where TEntity : DatabaseObject
    {
        if (entity == null) return null!;

        var tracked = _context.Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
        if (tracked != null)
            return tracked;

        _context.Attach(entity);
        return entity;
    }
}