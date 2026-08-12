using Microsoft.EntityFrameworkCore;
using OrderSystemManagment.Domain.Entities;
using OrderSystemManagment.Domain.Interfaces;
using OrderSystemManagment.DataAccess.Context;

namespace OrderSystemManagment.DataAccess.Repositories;

public class LookupRepository : ILookupRepository
{
    private readonly OrderDbContext _context;

    public LookupRepository(OrderDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<CurrencyType>> GetCurrencyTypesAsync()
    {
        return await _context.CurrencyTypes
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetItemsAsync()
    {
        return await _context.Items
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Include(x => x.Warehouse)
            .ToListAsync();
    }

    public async Task<IEnumerable<Warehouse>> GetWarehousesAsync()
    {
        return await _context.Warehouses
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }
}