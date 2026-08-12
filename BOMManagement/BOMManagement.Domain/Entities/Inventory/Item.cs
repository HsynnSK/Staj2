using BOMManagement.Domain.Entities.BaseModels;
using BOMManagement.Domain.Entities.BOMs;
using BOMManagement.Domain.Entities.Production;
using BOMManagement.Domain.Enums;

namespace BOMManagement.Domain.Entities.Items;

public class Item : DatabaseObject
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public UnitType MainUnit { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal StockQuantity { get; set; }

    public virtual Warehouse? DefaultWarehouse { get; set; }
    public virtual MaterialType MaterialType { get; set; } = null!; //MalzemeTypeId
    public virtual ICollection<BOMHeader>? Recipes { get; set; }
    public virtual WorkOrder? WorkOrder { get; set; }
}
