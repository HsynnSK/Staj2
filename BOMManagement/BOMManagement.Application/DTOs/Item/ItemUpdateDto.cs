using BOMManagement.Domain.Enums;

namespace BOMManagement.Application.DTOs.Item;

public class ItemUpdateDto
{
    public int Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public UnitType MainUnit { get; set; }
    public int? DefaultWarehouseId { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal StockQuantity { get; set; }
    public int MaterialTypeId { get; set; }
}
