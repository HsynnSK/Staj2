using OrderSystemManagment.Domain.Entities.BaseModels;
using System.Diagnostics.Contracts;

namespace OrderSystemManagment.Domain.Entities;

public class Item : DatabaseObject
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string GroupCode { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public string UnitOfMeasure { get; set; } = string.Empty;

    public Warehouse? Warehouse { get; set; }//WarehouseId

}