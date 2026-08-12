using BOMManagement.Domain.Entities.BaseModels;

namespace BOMManagement.Domain.Entities.Items;

public class Warehouse : DatabaseObject
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
}
