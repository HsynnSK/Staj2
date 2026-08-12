using OrderSystemManagment.Domain.Entities;

namespace OrderSystemManagment.Domain.Interfaces;

public interface ILookupRepository
{
    Task<IEnumerable<Customer>> GetCustomersAsync();

    Task<IEnumerable<CurrencyType>> GetCurrencyTypesAsync();

    Task<IEnumerable<Item>> GetItemsAsync();

    Task<IEnumerable<Warehouse>> GetWarehousesAsync();

}